namespace MyTrails.Importer
{
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Wta;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Creates new <see cref="Trail"/> instances from an imported <see cref="WtaTrail"/>.
    /// </summary>
    public interface ITrailFactory
    {
        /// <summary>
        /// Create a new <see cref="Trail"/> based on an existing trail.
        /// </summary>
        /// <param name="wtaTrail">The imported WTA trail to use for trail creating.</param>
        /// <param name="context">Trail context data.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        Trail CreateTrail(WtaTrail wtaTrail, MyTrailsContext context);

        /// <summary>
        /// Update an existing <see cref="Trail"/> with data from WTA.
        /// </summary>
        /// <param name="trail">The trail to update.</param>
        /// <param name="wtaTrail">The imported WTA  trail to use for updates.</param>
        /// <param name="context">Trail context data.</param>
        void UpdateTrail(Trail trail, WtaTrail wtaTrail, MyTrailsContext context);
    }
}
