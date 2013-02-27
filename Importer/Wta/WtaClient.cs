namespace MyTrails.Importer.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for communicating with wta.org
    /// </summary>
    public class WtaClient : IWtaClient
    {
        /// <summary>
        /// Fetch trail definitions from WTA.
        /// </summary>
        /// <returns>A sequence of trails from WTA.</returns>
        /// <seealso cref="IWtaClient.FetchTrails"/>
        public Task<IEnumerable<WtaTrail>> FetchTrails()
        {
            throw new NotImplementedException();
        }
    }
}
