namespace MyTrails.ServiceLib.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;

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

        /// <summary>
        /// Build the retry policy to use when querying WTA.
        /// </summary>
        /// <param name="logger">Logging interface to log retries to.</param>
        /// <returns>An initialized retry policy.</returns>
        /// <seealso cref="IWtaClient.BuildRetryPolicy"/>
        RetryPolicy BuildRetryPolicy(ILog logger);
    }
}
