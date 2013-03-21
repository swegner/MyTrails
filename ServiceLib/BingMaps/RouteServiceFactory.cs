namespace MyTrails.ServiceLib.BingMaps
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using MyTrails.ServiceLib.BingMaps.Routing;

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
        /// <remarks>Consumer is responsible for disposing of returned <see cref="IRouteService"/> instance.</remarks>
        /// <seealso cref="IRouteServiceFactory.CreateRouteService"/>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Consumer will dispose of instance.")]
        public IRouteService CreateRouteService()
        {
            return new RouteServiceClient("BasicHttpBinding_IRouteService");
        }
    }
}