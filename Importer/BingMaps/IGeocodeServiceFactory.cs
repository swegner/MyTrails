namespace MyTrails.Importer.BingMaps
{
    using MyTrails.Importer.BingMaps.Geocoding;

    /// <summary>
    /// Interface for creating <see cref="IGeocodeService"/> instnaces.
    /// </summary>
    public interface IGeocodeServiceFactory
    {
        /// <summary>
        /// Create a new <see cref="IGeocodeService"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IGeocodeService"/> instance.</returns>
        IGeocodeService CreateGeocodeService(); 
    }
}
