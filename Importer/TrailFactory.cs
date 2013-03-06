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
        /// Dictionary of required passes, keyed by description.
        /// </summary>
        private Dictionary<string, RequiredPass> _passDictionary;

        /// <summary>
        /// Dictionary of trail features, keyed by WTA ID.
        /// </summary>
        private Dictionary<WtaFeatures, TrailFeature> _trailFeatureDictionary;

        /// <summary>
        /// Dictionary of trail characteristics, keyed by WTA ID.
        /// </summary>
        private Dictionary<WtaUserInfo, TrailCharacteristic> _characteristicDictionary; 

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
        /// <param name="passes">Sequence of required passes registered.</param>
        /// <param name="trailFeatures">Sequence of registered trail features.</param>
        /// <param name="trailCharacteristics">Sequence of registered trail characteristics.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        /// <seealso cref="ITrailFactory.CreateTrail"/>
        public Trail CreateTrail(WtaTrail wtaTrail, IEnumerable<Region> regions,
            IEnumerable<Guidebook> guidebooks, IEnumerable<RequiredPass> passes,
            IEnumerable<TrailFeature> trailFeatures, IEnumerable<TrailCharacteristic> trailCharacteristics)
        {
            if (wtaTrail == null)
            {
                throw new ArgumentNullException("wtaTrail");
            }

            this.InitializeCaches(regions, guidebooks, passes, trailFeatures, trailCharacteristics);

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

            RequiredPass requiredPass = !string.IsNullOrEmpty(wtaTrail.RequiredPass) ?
                this._passDictionary[wtaTrail.RequiredPass] :
                null;

            IEnumerable<string> photoLinks = wtaTrail.Photos
                .Select(u => u.AbsoluteUri);

            IEnumerable<TrailFeature> features = this._trailFeatureDictionary
                .Where(kvp => wtaTrail.Statistics.Features.HasFlag(kvp.Key))
                .Select(kvp => kvp.Value);

            IEnumerable<TrailCharacteristic> characteristics = this._characteristicDictionary
                .Where(kvp => wtaTrail.Statistics.UserInfo.HasFlag(kvp.Key))
                .Select(kvp => kvp.Value);

            Trail trail = new Trail
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
                RequiredPass = requiredPass,
            };

            foreach (TrailFeature feature in features)
            {
                trail.Features.Add(feature);
            }

            foreach (TrailCharacteristic characteristic in characteristics)
            {
                trail.Characteristics.Add(characteristic);
            }

            foreach (string link in photoLinks)
            {
                trail.PhotoLinks.Add(link);
            }

            return trail;
        }

        /// <summary>
        /// Initialize lookup dictionaries for registered regions and guidebooks.
        /// </summary>
        /// <param name="regions">Registered regions to enumerate.</param>
        /// <param name="guideBooks">Sequence of registered guidebooks.</param>
        /// <param name="passes">Sequence of registered passes.</param>
        /// <param name="trailFeatures">Sequence of registered trail features.</param>
        /// <param name="characteristics">Sequence of registered trail characteristics.</param>
        private void InitializeCaches(IEnumerable<Region> regions, IEnumerable<Guidebook> guideBooks, 
            IEnumerable<RequiredPass> passes, IEnumerable<TrailFeature> trailFeatures, IEnumerable<TrailCharacteristic> characteristics)
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

                        this.Logger.Debug("Initializing required passes dictionary.");
                        this._passDictionary = passes.ToDictionary(rp => rp.Description, rp => rp);

                        this.Logger.Debug("Initializing trail features dictionary.");
                        this._trailFeatureDictionary = trailFeatures.ToDictionary(tf => (WtaFeatures)tf.WtaId, tf => tf);

                        this.Logger.Debug("Initializing trail characteristics dictionary.");
                        this._characteristicDictionary = characteristics.ToDictionary(tc => (WtaUserInfo)tc.WtaId, tc => tc);

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