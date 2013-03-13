namespace MyTrails.Importer.BingMaps
{
    using MyTrails.Importer.BingMaps.Routing;

    /// <summary>
    /// Factory for creating <see cref="IRouteService"/> instances.
    /// </summary>
    public interface IRouteServiceFactory
    {
        /// <summary>
        /// Create a new <see cref="IRouteService"/> instance.
        /// </summary>
        /// <returns>A new <see cref=" IRouteService"/> instance.</returns>
        /// <remarks>Consumer is responsible for disposing of returned <see cref="IRouteService"/> instance.</remarks>
        IRouteService CreateRouteService();
    }
}
