namespace MyTrails.Importer
{
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
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        Trail CreateTrail(WtaTrail wtaTrail);
    }
}
