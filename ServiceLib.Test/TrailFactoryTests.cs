namespace MyTrails.ServiceLib.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data.Spatial;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.ServiceLib.Test.Logging;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Unit tests for the <see cref="TrailFactory"/> class.
    /// </summary>
    [TestClass]
    public class TrailFactoryTests
    {
        /// <summary>
        /// Sample guidebook to use during testing.
        /// </summary>
        private static readonly Guidebook AnyGuidebook = new Guidebook
        {
            Author = "Any Author",
            Title = "Any Guidebook Title",
        };

        /// <summary>
        /// Test trail data to use in factory tests.
        /// </summary>
        private TestData _trailData;

        /// <summary>
        /// <see cref="TrailFactory"/> instance to test against.
        /// </summary>
        private TrailFactory _factory;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            const string anyWtaId = "any-wta-id";
            const string anyTrailTitle = "Any Trail Title";
            const double anyRating = 4.345;
            const double anyMileage = 345.213;
            const double anyElevation = 9834.123;
            const double anyHighPoint = 2353.22;
            const string anyTrailPhotoLink = "http://anytrail/photo/link";
            
            Uri anyTrailUrl = new Uri("http://any/trail/url");
            DbGeography anyLocation = DbGeographyExt.PointFromCoordinates(23.456, -109.654);

            this._factory = new TrailFactory
            {
                Logger = new StubLog(),
            };

            Guidebook guidebook;
            Region region;
            RequiredPass requiredPass;
            TrailFeature trailFeature;
            TrailCharacteristic characteristic;
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                trailContext.ClearDatabase();
                trailContext.SaveChanges();

                trailContext.Guidebooks.Add(AnyGuidebook);
                trailContext.SaveChanges();

                guidebook = trailContext.Guidebooks
                    .Where(gb => gb.Title == AnyGuidebook.Title && gb.Author == AnyGuidebook.Author)
                    .First();
                region = trailContext.Regions
                    .First();
                requiredPass = trailContext.Passes
                    .First();
                trailFeature = trailContext.TrailFeatures
                    .First();
                characteristic = trailContext.TrailCharacteristics
                    .First();
            }

            this._trailData = new TestData
            {
                WtaTrail = new WtaTrail
                {
                    Uid = anyWtaId,
                    Title = anyTrailTitle,
                    Url = anyTrailUrl,
                    Location = new WtaLocation
                    {
                        Latitude = anyLocation.Latitude.Value,
                        Longitude = anyLocation.Longitude.Value,
                        RegionId = region.WtaId,
                    },
                    Statistics = new WtaStatistics
                    {
                        ElevationGain = anyElevation,
                        HighPoint = anyHighPoint,
                        Mileage = anyMileage,
                        Features = (WtaFeatures)trailFeature.WtaId,
                        UserInfo = (WtaUserInfo)characteristic.WtaId,
                    },
                    Rating = anyRating,
                    Guidebook = new WtaGuidebook
                    {
                        Author = AnyGuidebook.Author,
                        Title = AnyGuidebook.Title,
                        CoverImage = new Uri("http://any/coverimage/uri"),
                        Merchants =
                        {
                            new WtaGuidebookMerchant
                            {
                                Outlet = "any outlet",
                                Url = new Uri("http://any/merchant/uri"),
                            },
                        },
                    },
                    RequiredPass = requiredPass.Description,
                    Photos = { new Uri(anyTrailPhotoLink) },
                },
                OriginalTrail = new Trail
                {
                    WtaId = anyWtaId,
                    Name = anyTrailTitle,
                    Url = anyTrailUrl,
                },
                ExpectedOutput = new Trail
                {
                    WtaId = anyWtaId,
                    Name = anyTrailTitle,
                    Url = anyTrailUrl,
                    Location = anyLocation,
                    Region = region,
                    RegionId = region.Id,
                    WtaRating = anyRating,
                    ElevationGain = anyElevation,
                    HighPoint = anyHighPoint,
                    Mileage = anyMileage,
                    Guidebook = AnyGuidebook,
                    GuidebookId = guidebook.Id,
                    RequiredPass = requiredPass,
                    RequiredPassId = requiredPass.Id,
                    PhotoLinks = { anyTrailPhotoLink },
                    Features = { trailFeature },
                    Characteristics = { characteristic },
                }
            };
        }

        /// <summary>
        /// Clean up test helper objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                trailContext.ClearDatabase();
                trailContext.SaveChanges();
            }
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaId"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsWtaId()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.WtaId);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Name"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsName()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Name);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Url"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsUrl()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Url);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaRating"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRating()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.WtaRating);
        }

        /// <summary>
        /// Verify that the factory updates the <see cref="MyTrails.Contracts.Data.Trail.WtaRating"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesRating()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.WtaRating);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="MyTrails.Contracts.Data.Trail.Location"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsLocation()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Location, new DbGeographyPointComparer());
        }

        /// <summary>
        /// Verify that the factory updates <see cref="MyTrails.Contracts.Data.Trail.Location"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesLocation()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.Location, new DbGeographyPointComparer());
        }

        /// <summary>
        /// Verify that the factory is resilient to null <see cref="WtaTrail.Location"/> data.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void SkipsNullLocation()
        {
            // Arrange
            this._trailData.WtaTrail.Location = null;
            this._trailData.ExpectedOutput.Location = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Location);
        }

        /// <summary>
        /// Verify that the factory is resilient to null latitude / longitude
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullLatitudeLongitude()
        {
            // Arrange
            this._trailData.WtaTrail.Location.Latitude = null;
            this._trailData.WtaTrail.Location.Longitude = null;
            this._trailData.ExpectedOutput.Location = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Location);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Region"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRegion()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.RegionId);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.Region"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesRegion()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.RegionId);
        }

        /// <summary>
        /// Verify that the factory is resilient to null region
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullRegion()
        {
            // Arrange
            this._trailData.WtaTrail.Location.RegionId = null;
            this._trailData.ExpectedOutput.Region = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Region);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.ElevationGain"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsElevationGain()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.ElevationGain);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.ElevationGain"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesElevationGain()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.ElevationGain);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.ElevationGain"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullElevationGain()
        {
            // Arrange
            this._trailData.WtaTrail.Statistics.ElevationGain = null;
            this._trailData.ExpectedOutput.ElevationGain = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.ElevationGain);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Mileage"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsMileage()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Mileage);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.Mileage"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesMileage()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.Mileage);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.Mileage"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullMileage()
        {
            // Arrange
            this._trailData.WtaTrail.Statistics.Mileage = null;
            this._trailData.ExpectedOutput.Mileage = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Mileage);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.HighPoint"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsHighPoint()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.HighPoint);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.HighPoint"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesHighPoint()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.HighPoint);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.HighPoint"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullHighPoint()
        {
            // Arrange
            this._trailData.WtaTrail.Statistics.HighPoint = null;
            this._trailData.ExpectedOutput.HighPoint = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.HighPoint);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.HighPoint"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsGuidebook()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.GuidebookId);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.HighPoint"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesGuidebook()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.GuidebookId);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaTrail.Guidebook"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullGuidebook()
        {
            // Arrange
            this._trailData.WtaTrail.Guidebook = null;
            this._trailData.ExpectedOutput.Guidebook = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Guidebook);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.RequiredPass"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRequiredPass()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.RequiredPassId);
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.RequiredPass"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesRequiredPass()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.RequiredPassId);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaTrail.RequiredPass"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullRequiredPass()
        {
            // Arrange
            this._trailData.WtaTrail.RequiredPass = null;
            this._trailData.ExpectedOutput.RequiredPass = null;

            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.RequiredPass);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.PhotoLinks"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsPhotoLinks()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.PhotoLinks.FirstOrDefault());
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.PhotoLinks"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesPhotoLinks()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.PhotoLinks.FirstOrDefault());
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Features"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsFeatures()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Features, new ProjectionComparer<ICollection<TrailFeature>, ICollection<int>>(
                    tfs => tfs.Select(tf => tf.Id).ToList(), new CollectionComparer<int>()));
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.Features"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesFeatures()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.Features, new ProjectionComparer<ICollection<TrailFeature>, ICollection<int>>(
                    tfs => tfs.Select(tf => tf.Id).ToList(), new CollectionComparer<int>()));
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Characteristics"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsCharacteristics()
        {
            // Act / Assert
            this.TestCreateTrail(this._trailData, t => t.Characteristics,
                new ProjectionComparer<ICollection<TrailCharacteristic>, ICollection<int>>(
                    cs => cs.Select(c => c.Id).ToList(), new CollectionComparer<int>()));
        }

        /// <summary>
        /// Verify that the factory updates <see cref="Trail.Characteristics"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdatesCharacteristics()
        {
            // Act / Assert
            this.TestUpdateTrail(this._trailData, t => t.Characteristics,
                new ProjectionComparer<ICollection<TrailCharacteristic>, ICollection<int>>(
                    cs => cs.Select(c => c.Id).ToList(), new CollectionComparer<int>()));
        }

        /// <summary>
        /// Call <see cref="TrailFactory.CreateTrail"/> on the test data and compare the property
        /// selected in <see cref="propertySelector"/> to the expected value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to select for verification.</typeparam>
        /// <param name="testData">The test data set.</param>
        /// <param name="propertySelector">Method to select the output property to verify.</param>
        /// <param name="comparer">Equality comparer to use, or null to use the default comparer.</param>
        private void TestCreateTrail<TProperty>(TestData testData, Func<Trail, TProperty> propertySelector,
            IEqualityComparer<TProperty> comparer = null)
        {
            // Act
            Trail actual;
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                Trail newTrail = this._factory.CreateTrail(testData.WtaTrail, trailContext);
                trailContext.Trails.Add(newTrail);

                trailContext.SaveChanges();
                actual = trailContext.Trails.Find(newTrail.Id);

                // Assert
                this.VerifyTrailProperty(testData.ExpectedOutput, actual, propertySelector, comparer);
            }
        }

        /// <summary>
        /// Call <see cref="TrailFactory.UpdateTrail"/> on the test data and compare the property
        /// selected in <see cref="propertySelector"/> to the expected value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to select for verification.</typeparam>
        /// <param name="testData">The test data set.</param>
        /// <param name="propertySelector">Method to select the output property to verify.</param>
        /// <param name="comparer">Equality comparer to use, or null to use the default comparer.</param>
        private void TestUpdateTrail<TProperty>(TestData testData, Func<Trail, TProperty> propertySelector,
            IEqualityComparer<TProperty> comparer = null)
        {
            // Arrange
            int trailId;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.Trails.Add(testData.OriginalTrail);
                context.SaveChanges();

                trailId = testData.OriginalTrail.Id;
            }

            // Act
            Trail actual;
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                actual = trailContext.Trails.Find(trailId);
                this._factory.UpdateTrail(actual, testData.WtaTrail, trailContext);

                trailContext.SaveChanges();

                // Assert
                this.VerifyTrailProperty(testData.ExpectedOutput, actual, propertySelector, comparer);
            }
        }

        /// <summary>
        /// Compare the property selected in <see cref="propertySelector"/> to the expected value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to select for verification.</typeparam>
        /// <param name="expected">The expected test data set.</param>
        /// <param name="actual">The actual test output.</param>
        /// <param name="propertySelector">Method to select the output property to verify.</param>
        /// <param name="comparer">Equality comparer to use, or null to use the default comparer.</param>
        private void VerifyTrailProperty<TProperty>(Trail expected, Trail actual, Func<Trail, TProperty> propertySelector,
            IEqualityComparer<TProperty> comparer)
        {
            TProperty expectedProperty = propertySelector(expected);
            TProperty actualProperty = propertySelector(actual);
            if (comparer == null)
            {
                Assert.AreEqual(expectedProperty, actualProperty);
            }
            else
            {
                Assert.IsTrue(comparer.Equals(expectedProperty, actualProperty));
            }
        }

        /// <summary>
        /// Helper class to hold sample data sets.
        /// </summary>
        private class TestData
        {
            /// <summary>
            /// The input <see cref="WtaTrail"/>
            /// </summary>
            public WtaTrail WtaTrail { get; set; }

            /// <summary>
            /// Original trail to update.
            /// </summary>
            public Trail OriginalTrail { get; set; }

            /// <summary>
            /// The expected <see cref="Trail"/> output.
            /// </summary>
            public Trail ExpectedOutput { get; set; }
        }

        /// <summary>
        /// Comparer which compares the contained objects ni the collection.
        /// </summary>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        private class CollectionComparer<T> : EqualityComparer<ICollection<T>>
        {
            /// <summary>
            /// Check whether two collections have equal contents.
            /// </summary>
            /// <param name="x">The first collection.</param>
            /// <param name="y">The second collection.</param>
            /// <returns>True if the object contents are equal, or false otherwise.</returns>
            /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
            public override bool Equals(ICollection<T> x, ICollection<T> y)
            {
                if (x == null || y == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                return x.Count == y.Count && x.Intersect(y).Count() == x.Count;
            }

            /// <summary>
            /// Generate a hash-code for the collection.
            /// </summary>
            /// <param name="obj">The collection to generate a hash code for.</param>
            /// <returns>A hash code for the collection.</returns>
            /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
            public override int GetHashCode(ICollection<T> obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.Aggregate(seed: 13, func: (s, e) => s ^ e.GetHashCode());
            }
        }

        /// <summary>
        /// Comparer which projects objects to another type to utilize a comparer for that type.
        /// </summary>
        /// <typeparam name="TSource">The source type of the projection.</typeparam>
        /// <typeparam name="TTarget">The target type of the projection.</typeparam>
        private class ProjectionComparer<TSource, TTarget> : IEqualityComparer<TSource>
            where TSource : class
        {
            /// <summary>
            /// Function used to project to a new type.
            /// </summary>
            private readonly Func<TSource, TTarget> _projection;

            /// <summary>
            /// Equality comparer for the projected type.
            /// </summary>
            private readonly IEqualityComparer<TTarget> _innerComparer; 

            /// <summary>
            /// Construct a new <see cref="ProjectionComparer{TSource,TTarget}"/> instance.
            /// </summary>
            /// <param name="projection">The projection function.</param>
            /// <param name="innerComparer">Comparer for the projected type.</param>
            public ProjectionComparer(Func<TSource, TTarget> projection, IEqualityComparer<TTarget> innerComparer)
            {
                this._projection = projection;
                this._innerComparer = innerComparer;
            }

            /// <summary>
            /// Check whether two projected objects have equal contents.
            /// </summary>
            /// <param name="x">The first object.</param>
            /// <param name="y">The second objecte.</param>
            /// <returns>True if the object projections are equal, or false otherwise.</returns>
            /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
            public bool Equals(TSource x, TSource y)
            {
                if (x == null || y == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                return this._innerComparer.Equals(this._projection(x), this._projection(y));
            }

            /// <summary>
            /// Generate a hash-code for the projected object.
            /// </summary>
            /// <param name="obj">The projected object to generate a hash code for.</param>
            /// <returns>A hash code for the projection.</returns>
            /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
            public int GetHashCode(TSource obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return this._innerComparer.GetHashCode(this._projection(obj));
            }
        }
    }
}
