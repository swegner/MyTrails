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
                r => r.Name,
                new Region
                {
                    Name = "Mt. Rainier",
                    SubRegions =
                    {
                        new SubRegion { Name = "SW - Cayuse Pass / Steven's Canyon", WtaId = Guid.Parse("cc329f21ff637826168e61bc9db77d65"), },
                        new SubRegion { Name = "NE - Sunrise / White River", WtaId = Guid.Parse("c8814620167ff2c018e9b0d6e961f0c1"), },
                        new SubRegion { Name = "SE - Longmire / Paradise", WtaId = Guid.Parse("3b53cfc78db378ecf7599df0fa14a51c"), },
                        new SubRegion { Name = "NW - Carbon River / Mowich", WtaId = Guid.Parse("cbe4acbaa2c01f9a5dbf4deece4e6ad9"), },
                    },
                },
                new Region
                {
                    Name = "Central Cascades",
                    SubRegions =
                    {
                        new SubRegion { Name = "Stevens Pass - West", WtaId = Guid.Parse("637634387ca38685f89162475c7fc1d2"), },
                        new SubRegion { Name = "Leavenworth Area", WtaId = Guid.Parse("684278bc46c11ebe3c5b7212b6f8e486"), },
                        new SubRegion { Name = "Entiat Mountains", WtaId = Guid.Parse("2b0ca41464d9baca77ced16fb4d40760"), },
                        new SubRegion { Name = "Lake Chelan", WtaId = Guid.Parse("a57d0bba101880a5f17cde85a72b243d"), },
                        new SubRegion { Name = "Stevens Pass - East", WtaId = Guid.Parse("f6845d37f1edba9dc2bc8a75346f5bd5"), },
                        new SubRegion { Name = "Blewett Pass", WtaId = Guid.Parse("83c2ab06fbf236015c8848042f706d58"), },
                    },
                },
                new Region
                {
                    Name = "Issaquah Alps",
                    SubRegions =
                    {
                        new SubRegion { Name = "Tiger Mountain", WtaId = Guid.Parse("9f13d8a3fcd2e1ab7a5b5aaab5997a9e"), },
                        new SubRegion { Name = "Squak Mountain", WtaId = Guid.Parse("70056b3c13ba158deec7750ef9701a94"), },
                        new SubRegion { Name = "Cougar Mountain", WtaId = Guid.Parse("325fdb0c3072b1b9acca522fb9e69ec2"), },
                    },
                },
                new Region
                {
                    Name = "North Cascades",
                    SubRegions =
                    {
                        new SubRegion { Name = "West Slope", WtaId = Guid.Parse("895f3b262060960490a83f1b41eebd74"), },
                        new SubRegion { Name = "Ross Lake", WtaId = Guid.Parse("69ef505a1122a587e03404754e81837a"), },
                        new SubRegion { Name = "Chewuch", WtaId = Guid.Parse("c5a809777fbe5e54e7be58d9da32e770"), },
                        new SubRegion { Name = "Mountain Loop Highway", WtaId = Guid.Parse("6194b417d1ae41b1ecd0d297b3fd2dea"), },
                        new SubRegion { Name = "Mount Baker Highway", WtaId = Guid.Parse("5674705352f9b856f2df1da7cbb8e0b1"), },
                        new SubRegion { Name = "Sawtooth", WtaId = Guid.Parse("f1d0577ed07086f098f97ad7819684d7"), },
                        new SubRegion { Name = "Methow", WtaId = Guid.Parse("c5ddae2034c4737fb5e7ee71ddb54024"), },
                        new SubRegion { Name = "Methow Valley", WtaId = Guid.Parse("5952810559bc6d85f808011a53ea6fcf"), },
                        new SubRegion { Name = "Baker Lake", WtaId = Guid.Parse("edfb3d2f85e035847d8523ec5a1a5d0e"), },
                        new SubRegion { Name = "East Slope", WtaId = Guid.Parse("e8710e0c0c5b28f2e947a121b78f912c"), },
                        new SubRegion { Name = "Suiattle River", WtaId = Guid.Parse("632316810277e470eb4104d1e8eef141"), },
                        new SubRegion { Name = "North Cascades Highway", WtaId = Guid.Parse("b52b426625b55325e408adfacae3b6c5"), },
                    },
                },
                new Region
                {
                    Name = "South Cascades",
                    SubRegions =
                    {
                        new SubRegion { Name = "Chinook Pass - Enumclaw or Hwy 410 area", WtaId = Guid.Parse("883e708ab442592f904fd87c1c909f6b"), },
                        new SubRegion { Name = "Columbia Gorge", WtaId = Guid.Parse("74fe67d98acee7d0decda17c2441a4d2"), },
                        new SubRegion { Name = "Goat Rocks", WtaId = Guid.Parse("b1376aba679a6bf3d6402cf91f16a44e"), },
                        new SubRegion { Name = "Lewis River Region", WtaId = Guid.Parse("35adb6fb84290947f778381d9d24a470"), },
                        new SubRegion { Name = "Mt. St. Helens", WtaId = Guid.Parse("17dcd22410be73abfd45d2703e123a35"), },
                        new SubRegion { Name = "Indian Heaven / Trapper Creek", WtaId = Guid.Parse("faa0c8fcfb1af66f7123437d3f2bcf52"), },
                        new SubRegion { Name = "White Pass / Cowlitz River Valley", WtaId = Guid.Parse("6f227fc5711324cee6170aa6d4b52cec"), },
                        new SubRegion { Name = "Dark Divide", WtaId = Guid.Parse("ac28fa6c89800fca796a2e61b879f416"), },
                        new SubRegion { Name = "Mount Adams", WtaId = Guid.Parse("73b109ca9145e4433f3089a1789d29bf"), },
                    },
                },
                new Region
                {
                    Name = "Snoqualmie Pass",
                    SubRegions =
                    {
                        new SubRegion { Name = "North Bend Area", WtaId = Guid.Parse("db086e5e85941a02ae188f726f7e9e2c"), },
                        new SubRegion { Name = "Snoqualmie Pass", WtaId = Guid.Parse("5d45ee6e4b5b077d069382b6aac9d388"), },
                        new SubRegion { Name = "Salmon La Sac/Teanaway", WtaId = Guid.Parse("f06510bd295c2d640ee2594d1b7a2ff6"), },
                    },
                },
                new Region
                {
                    Name = "Eastern Washington",
                    SubRegions =
                    {
                        new SubRegion { Name = "Inland NW", WtaId = Guid.Parse("e8796a64a51e10550267088d78936c54"), },
                        new SubRegion { Name = "Pasayten", WtaId = Guid.Parse("425fd9e8fd7edb23fc53782f16c2ea05"), },
                        new SubRegion { Name = "Okanogan Highlands", WtaId = Guid.Parse("13f85e52bc49ca16390d364ead3e1181"), },
                        new SubRegion { Name = "Colville", WtaId = Guid.Parse("d305615b5db417f18661c5233d2ce950"), },
                        new SubRegion { Name = "Tri-Cities", WtaId = Guid.Parse("6febb0079f87770b5e790a71aafa3770"), },
                        new SubRegion { Name = "Palouse", WtaId = Guid.Parse("3eff611193d7d4b57590df1f40b48800"), },
                        new SubRegion { Name = "Potholes Region", WtaId = Guid.Parse("3a415482d61dc1f893288c1bcf5cd8ae"), },
                        new SubRegion { Name = "Kettle Range", WtaId = Guid.Parse("fe742c316d095b81d23d712efa977d3d"), },
                        new SubRegion { Name = "Yakima", WtaId = Guid.Parse("7a4397f2ffde7d0b490ff1ca77cceb9e"), },
                        new SubRegion { Name = "Spokane Area", WtaId = Guid.Parse("bec6f9858a88f32a0912ed21d9c63b51"), },
                        new SubRegion { Name = "Wenatchee", WtaId = Guid.Parse("b4be8a42f05d2054cbb2ca031b9a6a03"), },
                    },
                },
                new Region
                {
                    Name = "Puget Sound and Islands",
                    SubRegions =
                    {
                        new SubRegion { Name = "North Sound", WtaId = Guid.Parse("92528ff6af30075eec65f35159defc50"), },
                        new SubRegion { Name = "South Sound", WtaId = Guid.Parse("df2c2da1637452abe74a5d10837c2e03"), },
                        new SubRegion { Name = "San Juan Islands", WtaId = Guid.Parse("7e0a6ce03ba1204d6bd3fdf64d3ad805"), },
                        new SubRegion { Name = "Whidbey Island", WtaId = Guid.Parse("d9dddf65d66479f065d40c1aeac18da3"), },
                    },
                },
                new Region
                {
                    Name = "Olympics",
                    SubRegions =
                    {
                        new SubRegion { Name = "North", WtaId = Guid.Parse("e4421728558408ef04e0a46afb2aa7ea"), },
                        new SubRegion { Name = "West", WtaId = Guid.Parse("89d7eb4c12e19e68b5efd9b825899d4e"), },
                        new SubRegion { Name = "SW Washington", WtaId = Guid.Parse("f0095f8e8f394f4210f50999bf8abf2c"), },
                        new SubRegion { Name = "Coast", WtaId = Guid.Parse("6135b6a861b5ac0c9b17a2f9b60c9295"), },
                        new SubRegion { Name = "Kitsap Peninsula", WtaId = Guid.Parse("3b2e0197b8be6c19a273c919b3301405"), },
                        new SubRegion { Name = "East", WtaId = Guid.Parse("bfbc0abe0fd04783aaad717ea2699866"), },
                        new SubRegion { Name = "South", WtaId = Guid.Parse("3ca3cd096bfedde6ff95b0859278cc75"), },
                    },
                });
        }
    }
}
