namespace Importer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Importer.Test.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer;
    using MyTrails.Importer.Wta;
    using DbGeography = System.Data.Spatial.DbGeography;

    /// <summary>
    /// Unit tests for the <see cref="TrailFactory"/> class.
    /// </summary>
    [TestClass]
    public class TrailFactoryTests
    {
        /// <summary>
        /// Sample WTA ID to use during testing.
        /// </summary>
        private const string AnyWtaId = "any-wta-id";

        /// <summary>
        /// Sample trail title to use during testing.
        /// </summary>
        private const string AnyTrailTitle = "Any Trail Title";

        /// <summary>
        /// Sample trail rating to use during testing.
        /// </summary>
        private const double AnyWtaRating = 4.345;

        /// <summary>
        /// Sample GUID identifier for subregion test data.
        /// </summary>
        private static readonly Guid AnySubRegionGuid = Guid.NewGuid();

        /// <summary>
        /// Sample trail URL to use during testing.
        /// </summary>
        private static readonly Uri AnyTrailUrl = new Uri("http://any/trail/url");

        /// <summary>
        /// Sample trail coordinates to use during testing.
        /// </summary>
        private static readonly DbGeography AnyLocation = DbGeographyExt.PointFromCoordinates(23.456, -109.654);

        /// <summary>
        /// Test trail data to use in factory tests.
        /// </summary>
        private TestData _trailData;

        /// <summary>
        /// Sample regions collection to use during testing.
        /// </summary>
        private ICollection<Region> _regions;

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
            this._factory = new TrailFactory
            {
                Logger = new StubLog(),
            };

            Region region = new Region
            {
                Name = "Any Region Name", 
                SubRegions = { new Region { Name = "Any SubRegion Name", WtaId = AnySubRegionGuid, }, },
            };
            this._regions = new Collection<Region>
            {
                region,
                region.SubRegions.First(),
            };

            this._trailData = new TestData
            {
                Input = new WtaTrail
                {
                    Uid = AnyWtaId,
                    Title = AnyTrailTitle,
                    Url = AnyTrailUrl,
                    Location = new WtaLocation
                    {
                        Latitude = AnyLocation.Latitude.Value,
                        Longitude = AnyLocation.Longitude.Value,
                        RegionId = AnySubRegionGuid,
                    },
                    Rating = AnyWtaRating,
                },
                ExpectedOutput = new Trail
                {
                    WtaId = AnyWtaId,
                    Name = AnyTrailTitle,
                    Url = AnyTrailUrl,
                    Location = AnyLocation,
                    Region = this._regions.First().SubRegions.First(),
                    WtaRating = AnyWtaRating,
                }
            };
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaId"/>
        /// </summary>
        [TestMethod]
        public void AssignsWtaId()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.WtaId);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Name"/>
        /// </summary>
        [TestMethod]
        public void AssignsName()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Name);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.Url"/>
        /// </summary>
        [TestMethod]
        public void AssignsUrl()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Url);
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="MyTrails.Contracts.Data.Trail.WtaRating"/>
        /// </summary>
        [TestMethod]
        public void AssignsRating()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.WtaRating);
        }

        /// <summary>
        /// Verify that the factory assigns <see cref="MyTrails.Contracts.Data.Trail.Location"/>
        /// </summary>
        [TestMethod]
        public void AssignsLocation()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Location, new DbGeographyPointComparer());
        }

        /// <summary>
        /// Verify that the factory is resilient to null <see cref="WtaTrail.Location"/> data.
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void AssignsSubRegion()
        {
            // Act / Assert
            this.TestFactoryMethod(this._trailData, t => t.Region);
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
            Trail actual = this._factory.CreateTrail(testData.Input, this._regions);

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
    }
}
