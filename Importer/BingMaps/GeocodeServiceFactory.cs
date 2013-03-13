namespace MyTrails.Importer.BingMaps
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
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
        /// <remarks>Consumer is responsible for disposing of returned <see cref="IGeocodeService"/> instance.</remarks>
        /// <seealso cref="IGeocodeServiceFactory.CreateGeocodeService"/>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Consumer will dispose of instance.")]
        public IGeocodeService CreateGeocodeService()
        {
            return new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
        }
    }
}