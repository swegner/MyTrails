namespace MyTrails.Importer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Spatial;
    using System.Linq;
    using log4net;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Creates new <see cref="Trail"/> instances from an imported <see cref="WtaTrail"/>.
    /// </summary>
    [Export(typeof(ITrailFactory))]
    public class TrailFactory : ITrailFactory
    {
        /// <summary>
        /// Lock object used for synchronization during initialization.
        /// </summary>
        private readonly object _initLockObject = new object();

        /// <summary>
        /// Dictionary of registered subregions, keyed by their WTA ID.
        /// </summary>
        private Dictionary<Guid, SubRegion> _subRegionDictionary;

        /// <summary>
        /// Whether <see cref="_subRegionDictionary"/> has been initialized.
        /// </summary>
        private bool _subRegionsInitialized;

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Create a new <see cref="Trail"/> based on an existing trail.
        /// </summary>
        /// <param name="wtaTrail">The imported WTA trail to use for trail creating.</param>
        /// <param name="regions">Sequence of registered regions with IDs, to associate with the trial.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        /// <seealso cref="ITrailFactory.CreateTrail"/>
        public Trail CreateTrail(WtaTrail wtaTrail, IEnumerable<Region> regions)
        {
            if (wtaTrail == null)
            {
                throw new ArgumentNullException("wtaTrail");
            }

            this.InitializeSubRegions(regions);

            DbGeography location;
            SubRegion subRegion;
            if (wtaTrail.Location != null)
            {
                location = (!AlmostEqual(wtaTrail.Location.Latitude, 0) && !AlmostEqual(wtaTrail.Location.Longitude, 0)) ?
                    DbGeographyExt.PointFromCoordinates(wtaTrail.Location.Latitude, wtaTrail.Location.Longitude) :
                    null;

                subRegion = this._subRegionDictionary[wtaTrail.Location.RegionId];
            }
            else
            {
                location = null;
                subRegion = null;
            }

            return new Trail
            {
                Name = wtaTrail.Title,
                WtaId = wtaTrail.Uid,
                Url = wtaTrail.Url,
                Location = location,
                WtaRating = wtaTrail.Rating,
                SubRegion = subRegion,
            };
        }

        /// <summary>
        /// Determine whether two floating point numbers are almost equal.
        /// </summary>
        /// <param name="x">First number to compare.</param>
        /// <param name="y">Second number to compare.</param>
        /// <returns>True if the numbers are almost equal, of false otherwise.</returns>
        private static bool AlmostEqual(double x, double y)
        {
            const double epsilon = 1e-8;

            double delta = x > y ? x - y : y - x;
            return delta < epsilon;
        }

        /// <summary>
        /// Initialize a lookup dictionary of registered subregions, keyed by WTA ID.
        /// </summary>
        /// <param name="regions">Registered regions to enumerate.</param>
        private void InitializeSubRegions(IEnumerable<Region> regions)
        {
            if (!this._subRegionsInitialized)
            {
                lock (this._initLockObject)
                {
                    if (!this._subRegionsInitialized)
                    {
                        this.Logger.Debug("Initializing subregion dictionary.");
                        this._subRegionDictionary = regions
                            .SelectMany(r => r.SubRegions)
                            .ToDictionary(sr => sr.WtaId, sr => sr);

                        this._subRegionsInitialized = true;
                    }
                }
            }
        }
    }
}