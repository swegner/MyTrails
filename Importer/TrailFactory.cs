namespace MyTrails.Importer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity.Migrations;
    using System.Data.Spatial;
    using System.Linq;
    using log4net;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
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
        private Dictionary<Guid, int> _regionDictionary;

        /// <summary>
        /// Dictionary of registered guide books.
        /// </summary>
        private ConcurrentDictionary<GuideBookKey, int> _guideBookDictionary;

        /// <summary>
        /// Dictionary of required passes, keyed by description.
        /// </summary>
        private Dictionary<string, int> _passDictionary;

        /// <summary>
        /// Dictionary of trail features, keyed by WTA ID.
        /// </summary>
        private Dictionary<WtaFeatures, int> _trailFeatureDictionary;

        /// <summary>
        /// Dictionary of trail characteristics, keyed by WTA ID.
        /// </summary>
        private Dictionary<WtaUserInfo, int> _characteristicDictionary;

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
        /// <param name="context">Trail context data.</param>
        /// <returns>A new <see cref="Trail"/> instance.</returns>
        /// <seealso cref="ITrailFactory.CreateTrail"/>
        public Trail CreateTrail(WtaTrail wtaTrail, MyTrailsContext context)
        {
            if (wtaTrail == null)
            {
                throw new ArgumentNullException("wtaTrail");
            }

            Trail trail = new Trail
            {
                Name = wtaTrail.Title, 
                WtaId = wtaTrail.Uid, 
                Url = wtaTrail.Url, 
            };

            this.UpdateTrail(trail, wtaTrail, context);

            return trail;
        }

        /// <summary>
        /// Update an existing <see cref="Trail"/> with data from WTA.
        /// </summary>
        /// <param name="trail">The trail to update.</param>
        /// <param name="wtaTrail">The imported WTA  trail to use for updates.</param>
        /// <param name="context">Trail context data.</param>
        /// <seealso cref="ITrailFactory.UpdateTrail"/>
        public void UpdateTrail(Trail trail, WtaTrail wtaTrail, MyTrailsContext context)
        {
            this.InitializeCaches(context);

            this.ExtractLocation(wtaTrail, trail);
            this.ExtractRegion(wtaTrail, trail);
            this.ExtractRequiredPass(wtaTrail, trail);
            this.ExtractRating(wtaTrail, trail);
            this.ExtractElevationGain(wtaTrail, trail);
            this.ExtractMileage(wtaTrail, trail);
            this.ExtractHighPoint(wtaTrail, trail);
            this.ExtractGuidebook(wtaTrail, trail, context);
            this.ExtractPhotoLinks(wtaTrail, trail);
            this.ExtractTrailFeatures(wtaTrail, trail, context);
            this.ExtractTrailCharacteristics(wtaTrail, trail, context);
        }

        /// <summary>
        /// Initialize lookup dictionaries for registered regions and guidebooks.
        /// </summary>
        /// <param name="context">Trail context data.</param>
        private void InitializeCaches(MyTrailsContext context)
        {
            if (!this._cachesInitialized)
            {
                lock (this._initLockObject)
                {
                    if (!this._cachesInitialized)
                    {
                        this.Logger.Debug("Initializing region dictionary.");
                        this._regionDictionary = context.Regions.ToDictionary(sr => sr.WtaId, sr => sr.Id);

                        this.Logger.Debug("Initializing guidebook dictionary.");
                        Dictionary<GuideBookKey, int> dict = context.Guidebooks.ToDictionary(
                            gb => new GuideBookKey(gb), 
                            gb => gb.Id);
                        this._guideBookDictionary = new ConcurrentDictionary<GuideBookKey, int>(dict);

                        this.Logger.Debug("Initializing required passes dictionary.");
                        this._passDictionary = context.Passes.ToDictionary(rp => rp.Description, rp => rp.Id);

                        this.Logger.Debug("Initializing trail features dictionary.");
                        this._trailFeatureDictionary = context.TrailFeatures.ToDictionary(tf => (WtaFeatures)tf.WtaId, tf => tf.Id);

                        this.Logger.Debug("Initializing trail characteristics dictionary.");
                        this._characteristicDictionary = context.TrailCharacteristics.ToDictionary(tc => (WtaUserInfo)tc.WtaId, tc => tc.Id);

                        this._cachesInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Extract the latitude and longitude from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractLocation(WtaTrail wtaTrail, Trail trail)
        {
            WtaLocation wtaLocation = wtaTrail.Location ?? new WtaLocation();
            DbGeography location = wtaLocation.Latitude.HasValue && wtaLocation.Longitude.HasValue ?
                DbGeographyExt.PointFromCoordinates(wtaLocation.Latitude.Value, wtaLocation.Longitude.Value) :
                null;

            trail.Location = location;
        }

        /// <summary>
        /// Extract the region ID from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractRegion(WtaTrail wtaTrail, Trail trail)
        {
            WtaLocation wtaLocation = wtaTrail.Location ?? new WtaLocation();
            int? regionId = wtaLocation.RegionId.HasValue ?
                this._regionDictionary[wtaLocation.RegionId.Value] :
                (int?)null;

            trail.RegionId = regionId;
        }

        /// <summary>
        /// Extract the required pass ID from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractRequiredPass(WtaTrail wtaTrail, Trail trail)
        {
            int? requiredPassId = !string.IsNullOrEmpty(wtaTrail.RequiredPass) ?
                this._passDictionary[wtaTrail.RequiredPass] :
                (int?)null;

            trail.RequiredPassId = requiredPassId;
        }

        /// <summary>
        /// Extract the trail rating from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractRating(WtaTrail wtaTrail, Trail trail)
        {
            trail.WtaRating = wtaTrail.Rating;
        }

        /// <summary>
        /// Extract the elevation gain from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractElevationGain(WtaTrail wtaTrail, Trail trail)
        {
            trail.ElevationGain = wtaTrail.Statistics.ElevationGain;
        }

        /// <summary>
        /// Extract the mileage from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractMileage(WtaTrail wtaTrail, Trail trail)
        {
            trail.Mileage = wtaTrail.Statistics.Mileage;
        }

        /// <summary>
        /// Extract the trail high point from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractHighPoint(WtaTrail wtaTrail, Trail trail)
        {
            trail.HighPoint = wtaTrail.Statistics.HighPoint;
        }

        /// <summary>
        /// Extract the <see cref="Guidebook"/> ID from a <see cref="WtaTrail."/>, adding it to
        /// the context if it doesn't exist.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        /// <param name="context">The datastore context.</param>
        private void ExtractGuidebook(WtaTrail wtaTrail, Trail trail, MyTrailsContext context)
        {
            WtaGuidebook wtaGuidebook = wtaTrail.Guidebook;
            int? guidebookId;
            if (wtaGuidebook != null)
            {
                GuideBookKey key = new GuideBookKey(wtaGuidebook);
                guidebookId = this._guideBookDictionary.GetOrAdd(key, k =>
                {
                    Guidebook gb = new Guidebook
                    {
                        Author = k.Author,
                        Title = k.Title,
                    };
                    context.Guidebooks.AddOrUpdate(g => new { g.Author, g.Title }, gb);
                    context.SaveChanges();
                    return gb.Id;
                });
            }
            else
            {
                guidebookId = null;
            }

            trail.GuidebookId = guidebookId;
        }

        /// <summary>
        /// Extract photo links from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        private void ExtractPhotoLinks(WtaTrail wtaTrail, Trail trail)
        {
            List<string> photoLinks = wtaTrail.Photos
                .Select(u => u.AbsoluteUri)
                .ToList();

            List<string> photosToAdd = photoLinks
                .Except(trail.PhotoLinks)
                .ToList();

            List<string> photosToRemove = trail.PhotoLinks
                .Except(photoLinks)
                .ToList();

            foreach (string photoLink in photosToRemove)
            {
                trail.PhotoLinks.Remove(photoLink);
            }

            foreach (string photoLink in photosToAdd)
            {
                trail.PhotoLinks.Add(photoLink);
            }
        }

        /// <summary>
        /// Extract trail features from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        /// <param name="context">The datastore context.</param>
        private void ExtractTrailFeatures(WtaTrail wtaTrail, Trail trail, MyTrailsContext context)
        {
            List<TrailFeature> features = this._trailFeatureDictionary
                .Where(kvp => wtaTrail.Statistics.Features.HasFlag(kvp.Key))
                .Select(kvp => context.TrailFeatures.Find(kvp.Value))
                .ToList();

            List<TrailFeature> featuresToAdd = features
                .Except(trail.Features)
                .ToList();

            List<TrailFeature> featuresToRemove = trail.Features
                .Except(features)
                .ToList();

            foreach (TrailFeature feature in featuresToRemove)
            {
                trail.Features.Remove(feature);
            }

            foreach (TrailFeature feature in featuresToAdd)
            {
                trail.Features.Add(feature);
            }
        }

        /// <summary>
        /// Extract trail characteristics from a <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to extract from.</param>
        /// <param name="trail">The <see cref="Trail"/> to update.</param>
        /// <param name="context">The datastore context.</param>
        private void ExtractTrailCharacteristics(WtaTrail wtaTrail, Trail trail, MyTrailsContext context)
        {
            List<TrailCharacteristic> characteristics = this._characteristicDictionary
                .Where(kvp => wtaTrail.Statistics.UserInfo.HasFlag(kvp.Key))
                .Select(kvp => context.TrailCharacteristics.Find(kvp.Value))
                .ToList();

            List<TrailCharacteristic> characteristicsToAdd = characteristics
                .Except(trail.Characteristics)
                .ToList();

            List<TrailCharacteristic> characteristicsToRemove = trail.Characteristics
                .Except(characteristics)
                .ToList();

            foreach (TrailCharacteristic characteristic in characteristicsToRemove)
            {
                trail.Characteristics.Remove(characteristic);
            }

            foreach (TrailCharacteristic characteristic in characteristicsToAdd)
            {
                trail.Characteristics.Add(characteristic);
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