namespace MyTrails.Importer
{
    using System;
    using System.ComponentModel.Composition;
    using System.Data.Spatial;
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

            // TODO: Write test case for null location/ zero lat / long
            DbGeography location = DbGeographyExt.PointFromCoordinates(wtaTrail.Location.Latitude, wtaTrail.Location.Longitude);

            return new Trail
            {
                Name = wtaTrail.Title,
                WtaId = wtaTrail.Uid,
                Url = wtaTrail.Url,
                Location = location,
                WtaRating = wtaTrail.Rating,
                Region = wtaTrail.Location.RegionId.ToString(),
            };
        }
    }
}