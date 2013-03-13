namespace MyTrails.Importer.BingMaps
{
    using System.ComponentModel.Composition;
    using System.Configuration;

    /// <summary>
    /// Credential settings for Bing Maps API.
    /// </summary>
    [Export(typeof(IBingMapsCredentials))]
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
    }
}