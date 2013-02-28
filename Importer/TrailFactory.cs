namespace MyTrails.Importer
{
    using System;
    using System.ComponentModel.Composition;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Creates new <see cref="Trail"/> instances from an imported <see cref="WtaTrail"/>.
    /// </summary>
    [Export(typeof(ITrailFactory))]
    public class TrailFactory : ITrailFactory
    {
        /// <summary>
        /// Create a new <see cref="Trail"/> based on an existing trail.
        /// </summary>
        /// <param name="wtaTrail">The imported WTA trail to use for trail creating.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        /// <seealso cref="ITrailFactory.CreateTrail"/>
        public Trail CreateTrail(WtaTrail wtaTrail)
        {
            if (wtaTrail == null)
            {
                throw new ArgumentNullException("wtaTrail");
            }

            return new Trail { WtaId = wtaTrail.Uid, };
        }
    }
}