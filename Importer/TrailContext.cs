namespace MyTrails.Importer
{
    using System.Collections.Generic;
    using MyTrails.Contracts.Data;

    /// <summary>
    /// Context data useful while building a trail definition.
    /// </summary>
    public class TrailContext
    {
        /// <summary>
        /// Collection of registered regions.
        /// </summary>
        public IEnumerable<Region> Regions { get; private set; }

        /// <summary>
        /// Collection of registered guidebooks.
        /// </summary>
        public IEnumerable<Guidebook> Guidebooks { get; private set; }

        /// <summary>
        /// Collection of registered passes.
        /// </summary>
        public IEnumerable<RequiredPass> Passes { get; private set; }

        /// <summary>
        /// Collection of registered trail features.
        /// </summary>
        public IEnumerable<TrailFeature> TrailFeatures { get; set; }

        /// <summary>
        /// Collection of registered trail characteristics.
        /// </summary>
        public IEnumerable<TrailCharacteristic> TrailCharacteristics { get; set; }

        /// <summary>
        /// Create a new <see cref="TrailContext"/> instance.
        /// </summary>
        /// <param name="regions">Registered regions.</param>
        /// <param name="guidebooks">Registered guidebooks.</param>
        /// <param name="passes">Registered passes.</param>
        /// <param name="trailFeatures">Registered trail features.</param>
        /// <param name="trailCharacteristics">Registered trail characteristics.</param>
        /// <returns>An initialized <see cref="TrailContext"/> instance.</returns>
        public static TrailContext Create(IEnumerable<Region> regions,
            IEnumerable<Guidebook> guidebooks, IEnumerable<RequiredPass> passes,
            IEnumerable<TrailFeature> trailFeatures, IEnumerable<TrailCharacteristic> trailCharacteristics)
        {
            return new TrailContext
            {
                Regions = regions,
                Guidebooks = guidebooks,
                Passes = passes,
                TrailFeatures = trailFeatures,
                TrailCharacteristics = trailCharacteristics,
            };
        }
    }
}
