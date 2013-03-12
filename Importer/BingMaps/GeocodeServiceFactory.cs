namespace MyTrails.Importer.BingMaps
{
    using System.ComponentModel.Composition;
    using MyTrails.Importer.BingMaps.Geocoding;

    /// <summary>
    /// Interface for creating <see cref="IGeocodeService"/> instnaces.
    /// </summary>
    [Export(typeof(IGeocodeServiceFactory))]
    public class GeocodeServiceFactory : IGeocodeServiceFactory
    {
        /// <summary>
        /// Create a new <see cref="IGeocodeService"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IGeocodeService"/> instance.</returns>
        /// <seealso cref="IGeocodeServiceFactory.CreateGeocodeService"/>
        public IGeocodeService CreateGeocodeService()
        {
            return new GeocodeServiceClient();
        }
    }
}