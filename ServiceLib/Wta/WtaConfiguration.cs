namespace MyTrails.ServiceLib.Wta
{
    using System;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Configuration settings for WTA functionality.
    /// </summary>
    [Export(typeof(IWtaConfiguration))]
    public class WtaConfiguration : IWtaConfiguration
    {
        /// <summary>
        /// Maximum number of concurrent requests to send.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.MaxConcurrentRequests"/>
        public int MaxConcurrentRequests
        {
            get { return int.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.MaxConcurrentRequests"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Request timeout for the Search API.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.SearchTimeout"/>
        public TimeSpan SearchTimeout
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.SearchTimeout"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Request timeout for the TripReports API.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.TripReportsTimeout"/>
        public TimeSpan TripReportsTimeout
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.TripReportsTimeout"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Maximum number of retries for each web request.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.RetryCount"/>
        public int RetryCount
        {
            get { return int.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.RetryCount"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Minimum retry backoff time.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.RetryMinBackOff"/>
        public TimeSpan RetryMinBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.RetryMinBackOff"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Maximum retry backoff time.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.RetryMaxBackOff"/>
        public TimeSpan RetryMaxBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.RetryMaxBackOff"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Retry value to use when calculating random delay between retries.
        /// </summary>
        /// <seealso cref="IWtaConfiguration.RetryDeltaBackOff"/>
        public TimeSpan RetryDeltaBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.Wta.RetryDeltaBackOff"], CultureInfo.InvariantCulture); }
        }
    }
}