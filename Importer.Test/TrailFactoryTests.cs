namespace MyTrails.Importer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Spatial;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer;
    using MyTrails.Importer.Test.Logging;
    using MyTrails.Importer.Wta;

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
        /// Sample regions collection to use during testing.
        /// </summary>
        private ICollection<Region> _regions;

        /// <summary>
        /// Sample guidebook collection to use during testing.
        /// </summary>
        private ICollection<Guidebook> _guideBooks;

        /// <summary>
        /// Sample passes collection to use during testing.
        /// </summary>
        private ICollection<RequiredPass> _passes;

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
            const string anyPassDescription = "Any Pass Description";
            const string anyTrailPhotoLink = "http://anytrail/photo/link";
            
            Guid anySubRegionGuid = Guid.NewGuid();
            Uri anyTrailUrl = new Uri("http://any/trail/url");
            DbGeography anyLocation = DbGeographyExt.PointFromCoordinates(23.456, -109.654);

            this._factory = new TrailFactory
            {
                Logger = new StubLog(),
            };

            Region region = new Region
            {
                Name = "Any Region Name", 
                SubRegions = { new Region { Name = "Any SubRegion Name", WtaId = anySubRegionGuid, }, },
            };
            this._regions = new Collection<Region>
            {
                region,
                region.SubRegions.First(),
            };

            this._guideBooks = new Collection<Guidebook>();

            RequiredPass requiredPass = new RequiredPass { Name = "Any pass name", Description = anyPassDescription };
            this._passes = new Collection<RequiredPass> { requiredPass };

            this._trailData = new TestData
            {
                Input = new WtaTrail
                {
                    Uid = anyWtaId,
                    Title = anyTrailTitle,
                    Url = anyTrailUrl,
                    Location = new WtaLocation
                    {
                        Latitude = anyLocation.Latitude.Value,
                        Longitude = anyLocation.Longitude.Value,
                        RegionId = anySubRegionGuid,
                    },
                    Statistics = new WtaStatistics
                    {
                        ElevationGain = anyElevation,
                        HighPoint = anyHighPoint,
                        Mileage = anyMileage,
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
                    RequiredPass = anyPassDescription,
                    Photos = { new Uri(anyTrailPhotoLink) },
                },
                ExpectedOutput = new Trail
                {
                    WtaId = anyWtaId,
                    Name = anyTrailTitle,
                    Url = anyTrailUrl,
                    Location = anyLocation,
                    Region = this._regions.First().SubRegions.First(),
                    WtaRating = anyRating,
                    ElevationGain = anyElevation,
                    HighPoint = anyHighPoint,
                    Mileage = anyMileage,
                    Guidebook = AnyGuidebook,
                    RequiredPass = requiredPass,
                    PhotoLinks = { anyTrailPhotoLink },
                }
            };
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaId"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsWtaId()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.WtaId);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Name"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsName()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Name);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Url"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsUrl()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Url);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaRating"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRating()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.WtaRating);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="MyTrails.Contracts.Data.Trail.Location"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsLocation()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Location, new DbGeographyPointComparer());
        }

        /// <summary>
        /// Verify that the factory is resilient to null <see cref="WtaTrail.Location"/> data.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void SkipsNullLocation()
        {
            // Arrange
            this._trailData.Input.Location = null;
            this._trailData.ExpectedOutput.Location = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Location);
        }

        /// <summary>
        /// Verify that the factory is resilient to null latitude / longitude
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullLatitudeLongitude()
        {
            // Arrange
            this._trailData.Input.Location.Latitude = null;
            this._trailData.Input.Location.Longitude = null;
            this._trailData.ExpectedOutput.Location = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Location);
        }

        /// <summary>
        /// Verify that the factory is resilient to null region
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullRegion()
        {
            // Arrange
            this._trailData.Input.Location.RegionId = null;
            this._trailData.ExpectedOutput.Region = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Region);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Region"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRegion()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Region);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.ElevationGain"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsElevationGain()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.ElevationGain);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.ElevationGain"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullElevationGain()
        {
            // Arrange
            this._trailData.Input.Statistics.ElevationGain = null;
            this._trailData.ExpectedOutput.ElevationGain = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.ElevationGain);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.Mileage"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsMileage()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Mileage);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.Mileage"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullMileage()
        {
            // Arrange
            this._trailData.Input.Statistics.Mileage = null;
            this._trailData.ExpectedOutput.Mileage = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Mileage);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.HighPoint"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsHighPoint()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.HighPoint);
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaStatistics.HighPoint"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullHighPoint()
        {
            // Arrange
            this._trailData.Input.Statistics.HighPoint = null;
            this._trailData.ExpectedOutput.HighPoint = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.HighPoint);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.HighPoint"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsGuidebook()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Guidebook, new GuidebookComparer());
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaTrail.Guidebook"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullGuidebook()
        {
            // Arrange
            this._trailData.Input.Guidebook = null;
            this._trailData.ExpectedOutput.Guidebook = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Guidebook);
        }

        /// <summary>
        /// Verify that guidebook definitions are not duplicated.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void GuidebooksNotDuplicated()
        {
            // Arrange
            this._guideBooks.Add(AnyGuidebook);

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Guidebook);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.RequiredPass"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsRequiredPass()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.RequiredPass, new RequiredPassComparer());
        }

        /// <summary>
        /// Verify that the factory handles null <see cref="WtaTrail.RequiredPass"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void HandlesNullRequiredPass()
        {
            // Arrange
            this._trailData.Input.RequiredPass = null;
            this._trailData.ExpectedOutput.RequiredPass = null;

            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.RequiredPass);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="Trail.PhotoLinks"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AssignsPhotoLinks()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.PhotoLinks.FirstOrDefault());
        }

        /// <summary>
        /// Call <see cref="TrailFactory.CreateTrail"/> on the test data and compare the property
        /// selected in <see cref="propertySelector"/> to the expected value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to select for verification.</typeparam>
        /// <param name="testData">The test data set.</param>
        /// <param name="propertySelector">Method to select the output property to verify.</param>
        /// <param name="comparer">Equality comparer to use, or null to use the default comparer.</param>
        private void TestFactoryMethod<TProperty>(TestData testData, Func<Trail, TProperty> propertySelector,
            IEqualityComparer<TProperty> comparer = null)
        {
            // Arrange
            if (comparer == null)
            {
                comparer = EqualityComparer<TProperty>.Default;
            }

            // Act
            Trail actual = this._factory.CreateTrail(testData.Input, this._regions, this._guideBooks, this._passes);

            // Assert
            TProperty expectedProperty = propertySelector(testData.ExpectedOutput);
            TProperty actualProperty = propertySelector(actual);
            Assert.IsTrue(comparer.Equals(expectedProperty, actualProperty));
        }

        /// <summary>
        /// Helper class to hold sample data sets.
        /// </summary>
        private class TestData
        {
            /// <summary>
            /// The input <see cref="WtaTrail"/>
            /// </summary>
            public WtaTrail Input { get; set; }

            /// <summary>
            /// The expected <see cref="Trail"/> output.
            /// </summary>
            public Trail ExpectedOutput { get; set; }
        }

        /// <summary>
        /// Equality comparer to check two <see cref="DbGeography"/> points for equality.
        /// </summary>
        private class DbGeographyPointComparer : EqualityComparer<DbGeography>
        {
            /// <summary>
            /// Check whether two <see cref="DbGeography"/> points are equal.
            /// </summary>
            /// <param name="x">The first <see cref="DbGeography"/> point.</param>
            /// <param name="y">The second <see cref="DbGeography"/> point.</param>
            /// <returns>True if the points are equal, or false otherwise.</returns>
            /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
            public override bool Equals(DbGeography x, DbGeography y)
            {
                if (x == null || y == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                if (x.PointCount != 1 || y.PointCount != 1)
                {
                    throw new InvalidOperationException("Comparer only support single-point DbGeography objects.");
                }

                return object.ReferenceEquals(x, y) ||
                    (x.CoordinateSystemId == y.CoordinateSystemId &&
                    x.Latitude == y.Latitude);
            }

            /// <summary>
            /// Generate a hash-code for the <see cref="DbGeography"/> point.
            /// </summary>
            /// <param name="obj">The point to generate a hash code for.</param>
            /// <returns>A hash code for the object.</returns>
            /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
            public override int GetHashCode(DbGeography obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                if (obj.PointCount != 1)
                {
                    throw new InvalidOperationException("Comparer only support single-point DbGeography objects.");
                }

                int hash = 23;
                hash = (hash * obj.CoordinateSystemId.GetHashCode()) + 13;
                hash = (hash * obj.Latitude.GetHashCode()) + 13;
                hash = (hash * obj.Longitude.GetHashCode()) + 13;

                return hash;
            }
        }

        /// <summary>
        /// Equality comparer to check two <see cref="Guidebook"/> instances for equality.
        /// </summary>
        private class GuidebookComparer : EqualityComparer<Guidebook>
        {
            /// <summary>
            /// Check whether two <see cref="Guidebook"/> instances are equal.
            /// </summary>
            /// <param name="x">The first <see cref="Guidebook"/>.</param>
            /// <param name="y">The second <see cref="Guidebook"/>.</param>
            /// <returns>True if the objects are equal, or false otherwise.</returns>
            /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
            public override bool Equals(Guidebook x, Guidebook y)
            {
                if (x == null || y == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                return x.Author == y.Author && x.Title == y.Title;
            }

            /// <summary>
            /// Generate a hash-code for the <see cref="Guidebook"/> instance.
            /// </summary>
            /// <param name="obj">The object to generate a hash code for.</param>
            /// <returns>A hash code for the object.</returns>
            /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
            public override int GetHashCode(Guidebook obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                int hash = 23;
                hash = (hash * obj.Author.GetHashCode()) + 13;
                hash = (hash * obj.Title.GetHashCode()) + 13;

                return hash;
            }
        }

        /// <summary>
        /// Equality comparer to check two <see cref="RequiredPass"/> instances for equality.
        /// </summary>
        private class RequiredPassComparer : EqualityComparer<RequiredPass>
        {
            /// <summary>
            /// Check whether two <see cref="RequiredPass"/> instances are equal.
            /// </summary>
            /// <param name="x">The first <see cref="RequiredPass"/>.</param>
            /// <param name="y">The second <see cref="RequiredPass"/>.</param>
            /// <returns>True if the objects are equal, or false otherwise.</returns>
            /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
            public override bool Equals(RequiredPass x, RequiredPass y)
            {
                if (x == null || y == null)
                {
                    return object.ReferenceEquals(x, y);
                }

                return x.Name == y.Name && x.Description == y.Description;
            }

            /// <summary>
            /// Generate a hash-code for the <see cref="RequiredPass"/> instance.
            /// </summary>
            /// <param name="obj">The object to generate a hash code for.</param>
            /// <returns>A hash code for the object.</returns>
            /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
            public override int GetHashCode(RequiredPass obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                int hash = 23;
                hash = (hash * obj.Name.GetHashCode()) + 13;
                hash = (hash * obj.Description.GetHashCode()) + 13;

                return hash;
            }
        }
    }
}
