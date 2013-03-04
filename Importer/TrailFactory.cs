namespace MyTrails.Importer
{
    using System;
    using System.Collections.Concurrent;
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
        /// Dictionary of registered guide books.
        /// </summary>
        private ConcurrentDictionary<GuideBookKey, Guidebook> _guideBookDictionary;

        /// <summary>
        /// Whether entity caches has been initialized.
        /// </summary>
        private bool _cachesInitialized;

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
        /// <param name="guidebooks">Sequence of registered guidebooks.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        /// <seealso cref="ITrailFactory.CreateTrail"/>
        public Trail CreateTrail(WtaTrail wtaTrail, 
            IEnumerable<Region> regions,
            IEnumerable<Guidebook> guidebooks)
        {
            if (wtaTrail == null)
            {
                throw new ArgumentNullException("wtaTrail");
            }

            this.InitializeCaches(regions, guidebooks);

            WtaLocation wtaLocation = wtaTrail.Location ?? new WtaLocation();
            DbGeography location = wtaLocation.Latitude.HasValue && wtaLocation.Longitude.HasValue ?
                DbGeographyExt.PointFromCoordinates(wtaLocation.Latitude.Value, wtaLocation.Longitude.Value) :
                null;
            Region region = wtaLocation.RegionId.HasValue ?
                this._regionDictionary[wtaLocation.RegionId.Value] :
                null;

            WtaGuidebook wtaGuidebook = wtaTrail.Guidebook;
            Guidebook guidebook;
            if (wtaGuidebook != null)
            {
                GuideBookKey key = new GuideBookKey(wtaGuidebook);
                guidebook = this._guideBookDictionary.GetOrAdd(key, k => new Guidebook
                {
                    Author = k.Author,
                    Title = k.Title,
                });
            }
            else
            {
                guidebook = null;
            }

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
                Guidebook = guidebook,
            };
        }

        /// <summary>
        /// Initialize lookup dictionaries for registered regions and guidebooks.
        /// </summary>
        /// <param name="regions">Registered regions to enumerate.</param>
        /// <param name="guideBooks">Sequence of registered guidebooks.</param>
        private void InitializeCaches(IEnumerable<Region> regions, IEnumerable<Guidebook> guideBooks)
        {
            if (!this._cachesInitialized)
            {
                lock (this._initLockObject)
                {
                    if (!this._cachesInitialized)
                    {
                        this.Logger.Debug("Initializing region dictionary.");
                        this._regionDictionary = regions.ToDictionary(sr => sr.WtaId, sr => sr);

                        this.Logger.Debug("Initializing guidebook dictionary.");
                        Dictionary<GuideBookKey, Guidebook> dict = guideBooks.ToDictionary(
                            gb => new GuideBookKey(gb), 
                            gb => gb);
                        this._guideBookDictionary = new ConcurrentDictionary<GuideBookKey, Guidebook>(dict);

                        this._cachesInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Dictionary key to use to lookup existing guidebook definitions.
        /// </summary>
        private class GuideBookKey : Tuple<string, string>
        {
            /// <summary>
            /// Construct a new <see cref="GuideBookKey"/>
            /// </summary>
            /// <param name="guidebook">The guidebook to create a key for.</param>
            public GuideBookKey(WtaGuidebook guidebook)
                : this(guidebook.Author, guidebook.Title)
            {
            }

            /// <summary>
            /// Construct a new <see cref="GuideBookKey"/>
            /// </summary>
            /// <param name="guidebook">The guidebook to create a key for.</param>
            public GuideBookKey(Guidebook guidebook)
                : this(guidebook.Author, guidebook.Title)
            {
            }

            /// <summary>
            /// Construct a new <see cref="GuideBookKey"/>
            /// </summary>
            /// <param name="author">The guidebook author associated with the key.</param>
            /// <param name="title">THe guidebook title associated with the key.</param>
            private GuideBookKey(string author, string title)
                : base(author, title)
            {
            }

            /// <summary>
            /// The guidebook author associated with the key.
            /// </summary>
            public string Author
            {
                get { return this.Item1; }
            }

            /// <summary>
            /// The guidebook title associated with the key.
            /// </summary>
            public string Title
            {
                get { return this.Item2; }
            }
        }
    }
}