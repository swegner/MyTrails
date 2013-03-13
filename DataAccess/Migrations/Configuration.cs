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
        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals",
            Justification = "Seed data creates many local variables.")]
        private void SeedRegions(DbSet<Region> regions)
        {
            regions.AddOrUpdate(
                r => r.WtaId,
                new Region
                {
                    WtaId = Guid.Parse("344281caae0d5e845a5003400c0be9ef"),
                    Name = "Mt. Rainier",
                    SubRegions =
                    {
                        new Region { Name = "SW - Cayuse Pass / Steven's Canyon", WtaId = Guid.Parse("cc329f21ff637826168e61bc9db77d65"), },
                        new Region { Name = "NE - Sunrise / White River", WtaId = Guid.Parse("c8814620167ff2c018e9b0d6e961f0c1"), },
                        new Region { Name = "SE - Longmire / Paradise", WtaId = Guid.Parse("3b53cfc78db378ecf7599df0fa14a51c"), },
                        new Region { Name = "NW - Carbon River / Mowich", WtaId = Guid.Parse("cbe4acbaa2c01f9a5dbf4deece4e6ad9"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("b4845d8a21ad6a202944425c86b6e85f"),
                    Name = "Central Cascades",
                    SubRegions =
                    {
                        new Region { Name = "Stevens Pass - West", WtaId = Guid.Parse("637634387ca38685f89162475c7fc1d2"), },
                        new Region { Name = "Leavenworth Area", WtaId = Guid.Parse("684278bc46c11ebe3c5b7212b6f8e486"), },
                        new Region { Name = "Entiat Mountains", WtaId = Guid.Parse("2b0ca41464d9baca77ced16fb4d40760"), },
                        new Region { Name = "Lake Chelan", WtaId = Guid.Parse("a57d0bba101880a5f17cde85a72b243d"), },
                        new Region { Name = "Stevens Pass - East", WtaId = Guid.Parse("f6845d37f1edba9dc2bc8a75346f5bd5"), },
                        new Region { Name = "Blewett Pass", WtaId = Guid.Parse("83c2ab06fbf236015c8848042f706d58"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("592fcc9afd9208db3b81fdf93dada567"),
                    Name = "Issaquah Alps",
                    SubRegions =
                    {
                        new Region { Name = "Tiger Mountain", WtaId = Guid.Parse("9f13d8a3fcd2e1ab7a5b5aaab5997a9e"), },
                        new Region { Name = "Squak Mountain", WtaId = Guid.Parse("70056b3c13ba158deec7750ef9701a94"), },
                        new Region { Name = "Cougar Mountain", WtaId = Guid.Parse("325fdb0c3072b1b9acca522fb9e69ec2"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("49aff77512c523f32ae13d889f6969c9"),
                    Name = "North Cascades",
                    SubRegions =
                    {
                        new Region { Name = "West Slope", WtaId = Guid.Parse("895f3b262060960490a83f1b41eebd74"), },
                        new Region { Name = "Ross Lake", WtaId = Guid.Parse("69ef505a1122a587e03404754e81837a"), },
                        new Region { Name = "Chewuch", WtaId = Guid.Parse("c5a809777fbe5e54e7be58d9da32e770"), },
                        new Region { Name = "Mountain Loop Highway", WtaId = Guid.Parse("6194b417d1ae41b1ecd0d297b3fd2dea"), },
                        new Region { Name = "Mount Baker Highway", WtaId = Guid.Parse("5674705352f9b856f2df1da7cbb8e0b1"), },
                        new Region { Name = "Sawtooth", WtaId = Guid.Parse("f1d0577ed07086f098f97ad7819684d7"), },
                        new Region { Name = "Methow", WtaId = Guid.Parse("c5ddae2034c4737fb5e7ee71ddb54024"), },
                        new Region { Name = "Methow Valley", WtaId = Guid.Parse("5952810559bc6d85f808011a53ea6fcf"), },
                        new Region { Name = "Baker Lake", WtaId = Guid.Parse("edfb3d2f85e035847d8523ec5a1a5d0e"), },
                        new Region { Name = "East Slope", WtaId = Guid.Parse("e8710e0c0c5b28f2e947a121b78f912c"), },
                        new Region { Name = "Suiattle River", WtaId = Guid.Parse("632316810277e470eb4104d1e8eef141"), },
                        new Region { Name = "North Cascades Highway", WtaId = Guid.Parse("b52b426625b55325e408adfacae3b6c5"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("8a977ce4bf0528f4f833743e22acae5d"),
                    Name = "South Cascades",
                    SubRegions =
                    {
                        new Region { Name = "Chinook Pass - Enumclaw or Hwy 410 area", WtaId = Guid.Parse("883e708ab442592f904fd87c1c909f6b"), },
                        new Region { Name = "Columbia Gorge", WtaId = Guid.Parse("74fe67d98acee7d0decda17c2441a4d2"), },
                        new Region { Name = "Goat Rocks", WtaId = Guid.Parse("b1376aba679a6bf3d6402cf91f16a44e"), },
                        new Region { Name = "Lewis River Region", WtaId = Guid.Parse("35adb6fb84290947f778381d9d24a470"), },
                        new Region { Name = "Mt. St. Helens", WtaId = Guid.Parse("17dcd22410be73abfd45d2703e123a35"), },
                        new Region { Name = "Indian Heaven / Trapper Creek", WtaId = Guid.Parse("faa0c8fcfb1af66f7123437d3f2bcf52"), },
                        new Region { Name = "White Pass / Cowlitz River Valley", WtaId = Guid.Parse("6f227fc5711324cee6170aa6d4b52cec"), },
                        new Region { Name = "Dark Divide", WtaId = Guid.Parse("ac28fa6c89800fca796a2e61b879f416"), },
                        new Region { Name = "Mount Adams", WtaId = Guid.Parse("73b109ca9145e4433f3089a1789d29bf"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("04d37e830680c65b61df474e7e655d64"),
                    Name = "Snoqualmie Pass",
                    SubRegions =
                    {
                        new Region { Name = "North Bend Area", WtaId = Guid.Parse("db086e5e85941a02ae188f726f7e9e2c"), },
                        new Region { Name = "Snoqualmie Pass", WtaId = Guid.Parse("5d45ee6e4b5b077d069382b6aac9d388"), },
                        new Region { Name = "Salmon La Sac/Teanaway", WtaId = Guid.Parse("f06510bd295c2d640ee2594d1b7a2ff6"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("9d321b42e903a3224fd4fef44af9bee3"),
                    Name = "Eastern Washington",
                    SubRegions =
                    {
                        new Region { Name = "Inland NW", WtaId = Guid.Parse("e8796a64a51e10550267088d78936c54"), },
                        new Region { Name = "Pasayten", WtaId = Guid.Parse("425fd9e8fd7edb23fc53782f16c2ea05"), },
                        new Region { Name = "Okanogan Highlands", WtaId = Guid.Parse("13f85e52bc49ca16390d364ead3e1181"), },
                        new Region { Name = "Colville", WtaId = Guid.Parse("d305615b5db417f18661c5233d2ce950"), },
                        new Region { Name = "Tri-Cities", WtaId = Guid.Parse("6febb0079f87770b5e790a71aafa3770"), },
                        new Region { Name = "Palouse", WtaId = Guid.Parse("3eff611193d7d4b57590df1f40b48800"), },
                        new Region { Name = "Potholes Region", WtaId = Guid.Parse("3a415482d61dc1f893288c1bcf5cd8ae"), },
                        new Region { Name = "Kettle Range", WtaId = Guid.Parse("fe742c316d095b81d23d712efa977d3d"), },
                        new Region { Name = "Yakima", WtaId = Guid.Parse("7a4397f2ffde7d0b490ff1ca77cceb9e"), },
                        new Region { Name = "Spokane Area", WtaId = Guid.Parse("bec6f9858a88f32a0912ed21d9c63b51"), },
                        new Region { Name = "Wenatchee", WtaId = Guid.Parse("b4be8a42f05d2054cbb2ca031b9a6a03"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("0c1d82b18f8023acb08e4daf03173e94"),
                    Name = "Puget Sound and Islands",
                    SubRegions =
                    {
                        new Region { Name = "North Sound", WtaId = Guid.Parse("92528ff6af30075eec65f35159defc50"), },
                        new Region { Name = "South Sound", WtaId = Guid.Parse("df2c2da1637452abe74a5d10837c2e03"), },
                        new Region { Name = "San Juan Islands", WtaId = Guid.Parse("7e0a6ce03ba1204d6bd3fdf64d3ad805"), },
                        new Region { Name = "Whidbey Island", WtaId = Guid.Parse("d9dddf65d66479f065d40c1aeac18da3"), },
                    },
                },
                new Region
                {
                    WtaId = Guid.Parse("922e688d784aa95dfb80047d2d79dcf6"),
                    Name = "Olympics",
                    SubRegions =
                    {
                        new Region { Name = "North", WtaId = Guid.Parse("e4421728558408ef04e0a46afb2aa7ea"), },
                        new Region { Name = "West", WtaId = Guid.Parse("89d7eb4c12e19e68b5efd9b825899d4e"), },
                        new Region { Name = "SW Washington", WtaId = Guid.Parse("f0095f8e8f394f4210f50999bf8abf2c"), },
                        new Region { Name = "Coast", WtaId = Guid.Parse("6135b6a861b5ac0c9b17a2f9b60c9295"), },
                        new Region { Name = "Kitsap Peninsula", WtaId = Guid.Parse("3b2e0197b8be6c19a273c919b3301405"), },
                        new Region { Name = "East", WtaId = Guid.Parse("bfbc0abe0fd04783aaad717ea2699866"), },
                        new Region { Name = "South", WtaId = Guid.Parse("3ca3cd096bfedde6ff95b0859278cc75"), },
                    },
                });
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
                new TripType { WtaId = "Snowshoe-xc-ski", Description = "Snowshoe / Cross-country Ski" },
            });
        }
    }
}
