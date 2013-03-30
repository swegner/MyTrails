namespace MyTrails.ServiceLib.BingMaps
{
    using System;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Settings for Bing Maps API.
    /// </summary>
    [Export(typeof(IBingMapsConfiguration))]
    public class BingMapsConfiguration : IBingMapsConfiguration
    {
        /// <summary>
        /// The registered ApplicationID.
        /// </summary>
        /// <seealso cref="IBingMapsConfiguration.ApplicationId"/>
        public string ApplicationId
        { 
            get { return ConfigurationManager.AppSettings["MyTrails.ServiceLib.BingMaps.ApplicationId"]; }
        }

        /// <summary>
        /// Maximum number of retries for each web request.
        /// </summary>
        /// <seealso cref="IBingMapsConfiguration.RetryCount"/>
        public int RetryCount
        {
            get { return int.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.BingMaps.RetryCount"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Minimum retry backoff time.
        /// </summary>
        /// <seealso cref="IBingMapsConfiguration.RetryMinBackOff"/>
        public TimeSpan RetryMinBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.BingMaps.RetryMinBackOff"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Maximum retry backoff time.
        /// </summary>
        /// <seealso cref="IBingMapsConfiguration.RetryMaxBackOff"/>
        public TimeSpan RetryMaxBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.BingMaps.RetryMaxBackOff"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Retry value to use when calculating random delay between retries.
        /// </summary>
        /// <seealso cref="IBingMapsConfiguration.RetryDeltaBackOff"/>
        public TimeSpan RetryDeltaBackOff
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.BingMaps.RetryDeltaBackOff"], CultureInfo.InvariantCulture); }
        }
    }
}