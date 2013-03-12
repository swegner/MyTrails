namespace MyTrails.Importer.BingMaps
{
    using System.ComponentModel.Composition;
    using MyTrails.Importer.BingMaps.Routing;

    /// <summary>
    /// Factory for creating <see cref="IRouteService"/> instances.
    /// </summary>
    [Export(typeof(IRouteServiceFactory))]
    public class RouteServiceFactory : IRouteServiceFactory
    {
        /// <summary>
        /// Create a new <see cref="IRouteService"/> instance.
        /// </summary>
        /// <returns>A new <see cref=" IRouteService"/> instance.</returns>
        /// <seealso cref="IRouteServiceFactory.CreateRouteService"/>
        public IRouteService CreateRouteService()
        {
            return new RouteServiceClient();
        }
    }
}