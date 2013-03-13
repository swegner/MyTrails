namespace MyTrails.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;
    using MyTrails.Contracts.Data;

    /// <summary>
    /// Configuration class for database migrations.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Class instantiated via reflection by Entity Framework migrations.")]
    internal sealed class Configuration : DbMigrationsConfiguration<MyTrailsContext>
    {
        /// <summary>
        /// Construct a new <see cref="Configuration"/> instance.
        /// </summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        /// <summary>
        /// Seed a new database with data.
        /// </summary>
        /// <param name="context">The database context.</param>
        protected override void Seed(MyTrailsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.SeedRegions(context.Regions);
            this.SeedPasses(context.Passes);
            this.SeedTrailFeatures(context.TrailFeatures);
            this.SeedTrailCharacteristics(context.TrailCharacteristics);
            this.SeedTripTypes(context.TripTypes);

            base.Seed(context);
        }

        /// <summary>
        /// Seed the <see cref="MyTrailsContext.Regions"/> data with predefined regions.
        /// </summary>
        /// <param name="regions">The <see cref="DbSet{Region}"/> to seed.</param>
        private void SeedRegions(DbSet<Region> regions)
        {
            regions.AddOrUpdate(
                r => r.WtaId,
                new Region { WtaId = Guid.Parse("344281caae0d5e845a5003400c0be9ef"), Name = "Mt. Rainier" },
                new Region { WtaId = Guid.Parse("b4845d8a21ad6a202944425c86b6e85f"), Name = "Central Cascades" },
                new Region { WtaId = Guid.Parse("592fcc9afd9208db3b81fdf93dada567"), Name = "Issaquah Alps" },
                new Region { WtaId = Guid.Parse("49aff77512c523f32ae13d889f6969c9"), Name = "North Cascades" },
                new Region { WtaId = Guid.Parse("8a977ce4bf0528f4f833743e22acae5d"), Name = "South Cascades" },
                new Region { WtaId = Guid.Parse("04d37e830680c65b61df474e7e655d64"), Name = "Snoqualmie Pass" },
                new Region { WtaId = Guid.Parse("9d321b42e903a3224fd4fef44af9bee3"), Name = "Eastern Washington" },
                new Region { WtaId = Guid.Parse("0c1d82b18f8023acb08e4daf03173e94"), Name = "Puget Sound and Islands" },
                new Region { WtaId = Guid.Parse("922e688d784aa95dfb80047d2d79dcf6"), Name = "Olympics", });
        }

        /// <summary>
        /// Seed the <see cref="MyTrailsContext.Passes"/> data with predefined passes.
        /// </summary>
        /// <param name="passes">The <see cref="DbSet{TEntity}"/> to seed.</param>
        private void SeedPasses(DbSet<RequiredPass> passes)
        {
            passes.AddOrUpdate(p => p.Name, new[]
            {
                new RequiredPass { Name = "None", Description = "No pass or permit" },
                new RequiredPass { Name = "Northwest Forest Pass", Description = "Northwest Forest Pass" },
                new RequiredPass { Name = "Discover Pass", Description = "Discover Pass" },
                new RequiredPass { Name = "National Park", Description = "National Park/Refuge entry fee" },
                new RequiredPass { Name = "Sno-Park", Description = "Sno-Park pass" },
                new RequiredPass { Name = "Mount St. Helens", Description = "Mount St. Helens fee" },
            });
        }

        /// <summary>
        /// Seed the <see cref="MyTrailsContext.TrailFeatures"/> data with predefined features.
        /// </summary>
        /// <param name="features">The <see cref="DbSet{TEntity}"/> to seed.</param>
        private void SeedTrailFeatures(DbSet<TrailFeature> features)
        {
            features.AddOrUpdate(f => f.WtaId, new[]
            {
                new TrailFeature { WtaId = 1 << 0, Description = "Coast" },
                new TrailFeature { WtaId = 1 << 1, Description = "Rivers" },
                new TrailFeature { WtaId = 1 << 2, Description = "Lakes" },
                new TrailFeature { WtaId = 1 << 3, Description = "Waterfalls" },
                new TrailFeature { WtaId = 1 << 4, Description = "Old Growth" },
                new TrailFeature { WtaId = 1 << 5, Description = "Fall Foliage" },
                new TrailFeature { WtaId = 1 << 6, Description = "Wildflowers / Meadows" },
                new TrailFeature { WtaId = 1 << 7, Description = "Mountain Views" },
                new TrailFeature { WtaId = 1 << 8, Description = "Summits" },
                new TrailFeature { WtaId = 1 << 9, Description = "Wildlife" },
                new TrailFeature { WtaId = 1 << 10, Description = "Ridges / Passes" },
                new TrailFeature { WtaId = 1 << 11, Description = "Established Campsites" },
            });
        }

        /// <summary>
        /// Seed the <see cref="MyTrailsContext.TrailCharacteristics"/> data with predefined attributes.
        /// </summary>
        /// <param name="characteristics">The <see cref="DbSet{TEntity}"/> to seed.</param>
        private void SeedTrailCharacteristics(DbSet<TrailCharacteristic> characteristics)
        {
            characteristics.AddOrUpdate(tc => tc.WtaId, new[]
            {
                new TrailCharacteristic { WtaId = 1 << 0, Description = "Good for Kids" },
                new TrailCharacteristic { WtaId = 1 << 1, Description = "Dogs Allowed On-Leash" },
                new TrailCharacteristic { WtaId = 1 << 2, Description = "Dogs Allowed Without Leash" },
                new TrailCharacteristic { WtaId = 1 << 3, Description = "Dogs Not Allowed" },
                new TrailCharacteristic { WtaId = 1 << 4, Description = "May Encounter Pack Animals" },
                new TrailCharacteristic { WtaId = 1 << 5, Description = "May Encounter Mountain Bikes" },
                new TrailCharacteristic { WtaId = 1 << 6, Description = "May Encounter Motorized Vehicles" },
                new TrailCharacteristic { WtaId = 1 << 7, Description = "Permit or Pass Required" },
            });
        }

        /// <summary>
        /// Seed the <see cref="MyTrailsContext.TripTypes"/> data with predefined types.
        /// </summary>
        /// <param name="tripTypes">The <see cref="DbSet{TEntity}"/> to seed.</param>
        private void SeedTripTypes(DbSet<TripType> tripTypes)
        {
            tripTypes.AddOrUpdate(tt => tt.WtaId, new[]
            {
                new TripType { WtaId = "day-hike", Description = "Day Hike" },
                new TripType { WtaId = "overnight", Description = "Overnight Backpack" },
                new TripType { WtaId = "multi-night-backpack", Description = "Multi-Night Backpack" },
                new TripType { WtaId = "snowshoe-xc-ski", Description = "Snowshoe / Cross-country Ski" },
            });
        }
    }
}
