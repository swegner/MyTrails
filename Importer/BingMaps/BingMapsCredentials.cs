namespace MyTrails.Importer.BingMaps
{
    using System.Configuration;

    /// <summary>
    /// Credential settings for Bing Maps API.
    /// </summary>
    public class BingMapsCredentials : IBingMapsCredentials
    {
        /// <summary>
        /// The registered ApplicationID.
        /// </summary>
        /// <seealso cref="IBingMapsCredentials.ApplicationId"/>
        public string ApplicationId
        { 
            get { return ConfigurationManager.AppSettings["MyTrails.Importer.BingMaps.ApplicationId"]; }
        }

        /// <summary>
        /// The registered application token.
        /// </summary>
        /// <seealso cref="IBingMapsCredentials.Token"/>
        public string Token
        {
            get { return ConfigurationManager.AppSettings["MyTrails.Importer.BingMaps.Token"]; }
        }
    }
}