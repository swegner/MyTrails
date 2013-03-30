namespace MyTrails.ServiceLib.BingMaps
{
    using System;

    /// <summary>
    /// Settings for Bing Maps API.
    /// </summary>
    public interface IBingMapsConfiguration
    {
        /// <summary>
        /// The registered ApplicationID.
        /// </summary>
        string ApplicationId { get; }

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