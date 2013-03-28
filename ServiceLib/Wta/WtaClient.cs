namespace MyTrails.ServiceLib.Wta
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using MyTrails.ServiceLib.Retry;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for communicating with wta.org
    /// </summary>
    [Export(typeof(IWtaClient))]
    public class WtaClient : IWtaClient, IPartImportsSatisfiedNotification
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
            SearchEndpoint = new Uri(WtaEndpoint, "@@WindowsPhone/Search");
        }

        /// <summary>
        /// WTA configuration settings.
        /// </summary>
        [Import]
        public IWtaConfiguration Configuration { get; set; }

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
        /// Build the retry policy to use when querying WTA.
        /// </summary>
        /// <param name="logger">Logging interface to log retries to.</param>
        /// <returns>An initialized retry policy.</returns>
        /// <seealso cref="IWtaClient.BuildRetryPolicy"/>
        public RetryPolicy BuildRetryPolicy(ILog logger)
        {
            RetryStrategy strategy = new ExponentialBackoff(
                retryCount: this.Configuration.RetryCount,
                minBackoff: this.Configuration.RetryMinBackOff,
                maxBackoff: this.Configuration.RetryMaxBackOff,
                deltaBackoff: this.Configuration.RetryDeltaBackOff)
            {
                FastFirstRetry = true,
            };
            RetryPolicy policy = new RetryPolicy(new HttpErrorDetectionStrategy(), strategy);
            policy.Retrying += (sender, args) => logger.WarnFormat("Retrying WTA request due to exception: {0}", args.LastException);

            return policy;
        }

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
                httpClientResource.Resource.Timeout = this.Configuration.SearchTimeout;
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
                httpClientResource.Resource.Timeout = this.Configuration.TripReportsTimeout;
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
        /// Initialize data members from imported configuration.
        /// </summary>
        /// <seealso cref="IPartImportsSatisfiedNotification.OnImportsSatisfied"/>
        public void OnImportsSatisfied()
        {
            int maxConcurrency = this.Configuration.MaxConcurrentRequests;
            this._httpClientManager = new ConcurrentResourceManager<IHttpClient>(maxConcurrency);
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
