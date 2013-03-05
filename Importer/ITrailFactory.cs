namespace MyTrails.Importer
{
    using System.Collections.Generic;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Creates new <see cref="Trail"/> instances from an imported <see cref="WtaTrail"/>.
    /// </summary>
    public interface ITrailFactory
    {
        /// <summary>
        /// Create a new <see cref="Trail"/> based on an existing trail.
        /// </summary>
        /// <param name="wtaTrail">The imported WTA trail to use for trail creating.</param>
        /// <param name="regions">Sequence of registered regions with IDs, to associate with the trial.</param>
        /// <param name="guidebooks">Sequence of registered guidebooks.</param>
        /// <param name="passes">Sequence of required passes registered.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        Trail CreateTrail(WtaTrail wtaTrail, IEnumerable<Region> regions, IEnumerable<Guidebook> guidebooks,
            IEnumerable<RequiredPass> passes);
    }
}
