namespace MyTrails.Importer.BingMaps
{
    /// <summary>
    /// Credential settings for Bing Maps API.
    /// </summary>
    public interface IBingMapsCredentials
    {
        /// <summary>
        /// The registered ApplicationID.
        /// </summary>
        string ApplicationId { get; }
    }
}