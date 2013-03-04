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
        private Dictionary<Guid, Region> _regionDictionary;

        /// <summary>
        /// Whether <see cref="_regionDictionary"/> has been initialized.
        /// </summary>
        private bool _regionsInitialized;

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

            WtaLocation wtaLocation = wtaTrail.Location ?? new WtaLocation();
            DbGeography location = wtaLocation.Latitude.HasValue && wtaLocation.Longitude.HasValue ?
                DbGeographyExt.PointFromCoordinates(wtaLocation.Latitude.Value, wtaLocation.Longitude.Value) :
                null;
            Region region = wtaLocation.RegionId.HasValue ?
                this._regionDictionary[wtaLocation.RegionId.Value] :
                null;

            return new Trail
            {
                Name = wtaTrail.Title,
                WtaId = wtaTrail.Uid,
                Url = wtaTrail.Url,
                Location = location,
                WtaRating = wtaTrail.Rating,
                Region = region,
                ElevationGain = wtaTrail.Statistics.ElevationGain,
                Mileage = wtaTrail.Statistics.Mileage,
                HighPoint = wtaTrail.Statistics.HighPoint,
            };
        }

        /// <summary>
        /// Initialize a lookup dictionary of registered subregions, keyed by WTA ID.
        /// </summary>
        /// <param name="regions">Registered regions to enumerate.</param>
        private void InitializeSubRegions(IEnumerable<Region> regions)
        {
            if (!this._regionsInitialized)
            {
                lock (this._initLockObject)
                {
                    if (!this._regionsInitialized)
                    {
                        this.Logger.Debug("Initializing subregion dictionary.");
                        this._regionDictionary = regions.ToDictionary(sr => sr.WtaId, sr => sr);

                        this._regionsInitialized = true;
                    }
                }
            }
        }
    }
}