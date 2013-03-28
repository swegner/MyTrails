namespace MyTrails.ServiceLib.Wta
{
    using System;

    /// <summary>
    /// Configuration settings for WTA functionality.
    /// </summary>
    public interface IWtaConfiguration
    {
        /// <summary>
        /// Maximum number of concurrent requests to send.
        /// </summary>
        int MaxConcurrentRequests { get; }

        /// <summary>
        /// Request timeout for the Search API.
        /// </summary>
        TimeSpan SearchTimeout { get; }

        /// <summary>
        /// Request timeout for the TripReports API.
        /// </summary>
        TimeSpan TripReportsTimeout { get; }

        /// <summary>
        /// Maximum number of retries for each web request.
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// Minimum retry backoff time.
        /// </summary>
        TimeSpan RetryMinBackOff { get; }

        /// <summary>
        /// Maximum retry backoff time.
        /// </summary>
        TimeSpan RetryMaxBackOff { get; }

        /// <summary>
        /// Retry value to use when calculating random delay between retries.
        /// </summary>
        TimeSpan RetryDeltaBackOff { get; }
    }
}
