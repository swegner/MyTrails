namespace MyTrails.Importer.Wta
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for communicating with wta.org
    /// </summary>
    [Export(typeof(IWtaClient))]
    public class WtaClient : IWtaClient
    {
        /// <summary>
        /// Format string to create trip report URIs to query from.
        /// </summary>
        public const string TripReportsEndpointFormat = "@@WindowsPhone/TripReports?id=";

        /// <summary>
        /// Base endpoint URI for the WTA service API.
        /// </summary>
        public static readonly Uri WtaEndpoint;

        /// <summary>
        /// WTA search endpoint.
        /// </summary>
        public static readonly Uri SearchEndpoint;

        /// <summary>
        /// Maximum number of HTTP requests to send to WTA at once.
        /// </summary>
        private const int ConcurrentHttpRequests = 25;

        /// <summary>
        /// Manager to control concurrent HTTP requests.
        /// </summary>
        private ConcurrentResourceManager<IHttpClient> _httpClientManager;

        /// <summary>
        /// Whether the object has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initialize static type data.
        /// </summary>
        static WtaClient()
        {
            WtaEndpoint = new Uri("http://www.wta.org");
            SearchEndpoint = new Uri(WtaEndpoint, "@@WindowsPhone/Search?StartIndex=0&EndIndex=50");
        }

        /// <summary>
        /// Construct a new <see cref="WtaClient"/> instance.
        /// </summary>
        public WtaClient()
        {
            this._httpClientManager = new ConcurrentResourceManager<IHttpClient>(ConcurrentHttpRequests);
        }

        /// <summary>
        /// Factory for creating <see cref="IHttpClient"/> instances. 
        /// </summary>
        [Import]
        public IHttpClientFactory HttpClientFactory { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Fetch trail definitions from WTA.
        /// </summary>
        /// <returns>A sequence of trails from WTA.</returns>
        /// <seealso cref="IWtaClient.FetchTrails"/>
        public async Task<IList<WtaTrail>> FetchTrails()
        {
            IList<WtaTrail> trails;
            using (ManagedConcurentResource<IHttpClient> httpClientResource = 
                await this._httpClientManager.ObtainResource(() => this.HttpClientFactory.CreateClient(SearchEndpoint)))
            {
                this.Logger.Info("Fetching new trails from WTA.");
                httpClientResource.Resource.Timeout = Timeout.InfiniteTimeSpan;
                using (HttpResponseMessage response = await httpClientResource.Resource.SendGetRequest())
                using (HttpResponseMessage successResponse = response.EnsureSuccessStatusCode())
                {
                    this.Logger.Debug("Received response, deserializing content stream.");
                    using (Stream stream = await successResponse.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            MissingMemberHandling = MissingMemberHandling.Error,
                        };
                        trails = serializer.Deserialize<IList<WtaTrail>>(jsonReader);
                    }
                }
            }

            this.Logger.Debug("Finished deserializing JSON response.");
            return trails;
        }

        /// <summary>
        /// Fetch trip reports for a given trail.
        /// </summary>
        /// <param name="wtaTrailId">The WTA trail ID to fetch for.</param>
        /// <returns>A collection of trip reports for the trail.</returns>
        /// <seealso cref="IWtaClient.FetchTripReports"/>
        public async Task<IList<WtaTripReport>> FetchTripReports(string wtaTrailId)
        {
            Uri trailReportUri = new Uri(WtaEndpoint, string.Format("{0}{1}", TripReportsEndpointFormat, wtaTrailId));

            IList<WtaTripReport> tripReports;
            using (ManagedConcurentResource<IHttpClient> httpClientResource =
                await this._httpClientManager.ObtainResource(() => this.HttpClientFactory.CreateClient(trailReportUri)))
            {
                httpClientResource.Resource.Timeout = TimeSpan.FromSeconds(30);
                this.Logger.InfoFormat("Fetching trip reports for trail: {0}", wtaTrailId);

                using (HttpResponseMessage response = await httpClientResource.Resource.SendGetRequest())
                using (HttpResponseMessage successResponse = response.EnsureSuccessStatusCode())
                {
                    using (Stream stream = await successResponse.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            MissingMemberHandling = MissingMemberHandling.Error,
                        };
                        tripReports = serializer.Deserialize<IList<WtaTripReport>>(jsonReader);
                    }
                }
            }

            return tripReports;
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <seealso cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <param name="disposing">Whether it is safe to reference maanged objects.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._httpClientManager.Dispose();
                    this._httpClientManager = null;
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Manages limited concurrent access to a resource type.
        /// </summary>
        /// <typeparam name="T">The type of resource to manage.</typeparam>
        private sealed class ConcurrentResourceManager<T> : IDisposable
            where T : class, IDisposable
        {
            /// <summary>
            /// Semaphore which counts accesses to the resource.
            /// </summary>
            private SemaphoreSlim _semaphore;

            /// <summary>
            /// Whether the object has been disposed of.
            /// </summary>
            private bool _disposed;

            /// <summary>
            /// Construct a new <see cref="ConcurrentResourceManager{T}"/> instance.
            /// </summary>
            /// <param name="maxConcurrency">Maximum concurrency level.</param>
            public ConcurrentResourceManager(int maxConcurrency)
            {
                this._semaphore = new SemaphoreSlim(maxConcurrency);
            }

            /// <summary>
            /// Obtain a reference to the resource.
            /// </summary>
            /// <param name="factoryMethod">Factory method for creating or obtaining the resource.</param>
            /// <returns>A managed reference to the resource.</returns>
            public async Task<ManagedConcurentResource<T>> ObtainResource(Func<T> factoryMethod)
            {
                ManagedConcurentResource<T> managedResource;
                await this._semaphore.WaitAsync();

                try
                {
                    T resource = factoryMethod();

                    try
                    {
                        managedResource = new ManagedConcurentResource<T>(this._semaphore, resource);
                    }
                    catch
                    {
                        resource.Dispose();
                        throw;
                    }
                }
                catch
                {
                    this._semaphore.Release();
                    throw;
                }

                return managedResource;
            }

            /// <summary>
            /// Dispose of the object.
            /// </summary>
            /// <seealso cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                if (!this._disposed)
                {
                    this._semaphore.Dispose();
                    this._semaphore = null;

                    this._disposed = true;
                }
            }
        }

        /// <summary>
        /// Wrapper object to manage checking in and checking out resource.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        private sealed class ManagedConcurentResource<T> : IDisposable
            where T : class, IDisposable
        {
            /// <summary>
            /// Whether the sempahore lock was obtained.
            /// </summary>
            private readonly SemaphoreSlim _semaphore;

            /// <summary>
            /// Whether the object has been disposed of.
            /// </summary>
            private bool _disposed;

            /// <summary>
            /// Construct a new <see cref="ManagedConcurentResource{T}"/> instance.
            /// </summary>
            /// <param name="semaphore">Semaphore to release on disposal.</param>
            /// <param name="resource">The resource to manage.</param>
            public ManagedConcurentResource(SemaphoreSlim semaphore, T resource)
            {
                this._semaphore = semaphore;
                this.Resource = resource;
            }

            /// <summary>
            /// The managed resource.
            /// </summary>
            public T Resource { get; private set; }

            /// <summary>
            /// Dispose of object resources.
            /// </summary>
            /// <seealso cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                if (!this._disposed)
                {
                    this._semaphore.Release();

                    this.Resource.Dispose();
                    this.Resource = null;

                    this._disposed = true;
                }
            }
        }
    }
}
