namespace MyTrails.Importer.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Interface for communicating with wta.org
    /// </summary>
    public interface IWtaClient : IDisposable
    {
        /// <summary>
        /// Fetch trail definitions from WTA.
        /// </summary>
        /// <returns>A sequence of trails from WTA.</returns>
        Task<IList<WtaTrail>> FetchTrails();

        /// <summary>
        /// Fetch trip reports for a given trail.
        /// </summary>
        /// <param name="wtaTrailId">The WTA trail ID to fetch for.</param>
        /// <returns>A collection of trip reports for the trail.</returns>
        Task<IList<WtaTripReport>> FetchTripReports(string wtaTrailId);
    }
}
