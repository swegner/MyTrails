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

        /// <summary>
        /// The registered application token.
        /// </summary>
        string Token { get; }
    }
}