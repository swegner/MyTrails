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
        /// The base domain address for the WTA website.
        /// </summary>
        private static readonly Uri WtaDomain = new Uri("http://www.wta.org");

        /// <summary>
        /// WTA search endpoint.
        /// </summary>
        private static readonly Uri SearchEndpoint = new Uri(WtaDomain,
#if DEBUG
            "@@WindowsPhone/Search"); // "?StartIndex=0&EndIndex=100");
#else
            "@@WindowsPhone/Search");
#endif

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
            using (HttpClient httpClient = new HttpClient { Timeout = Timeout.InfiniteTimeSpan, })
            {
                this.Logger.Info("Fetching new trails from WTA.");
                using (HttpResponseMessage response = await httpClient.GetAsync(SearchEndpoint))
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
    }
}
