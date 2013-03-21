namespace MyTrails.Importer.Test.Extenders
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Extenders;
    using MyTrails.Importer.Test.Logging;
    using MyTrails.ServiceLib.BingMaps;
    using MyTrails.ServiceLib.BingMaps.Routing;

    /// <summary>
    /// Unit tests for the <see cref="DrivingDistanceExtender"/> class.
    /// </summary>
    [TestClass]
    public class DrivingDistanceExtenderTests
    {
        /// <summary>
        /// The <see cref="DrivingDistanceExtender"/> instance to test against.
        /// </summary>
        private DrivingDistanceExtender _extender;

        /// <summary>
        /// Mock <see cref="IRouteService"/> to inject test behavior.
        /// </summary>
        private Mock<IRouteService> _routeServiceMock;

        /// <summary>
        /// Mock <see cref="IBingMapsCredentials"/> to inject test behavior.
        /// </summary>
        private Mock<IBingMapsCredentials> _credentialsMock;

        /// <summary>
        /// Sample address to use during testing.
        /// </summary>
        private Address _anyAddress;

        /// <summary>
        /// Sample trail to use during testing.
        /// </summary>
        private Trail _anyTrail;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearDatabase();

            this._anyAddress = new Address
            {
                Location = "any location string",
                Coordinate = DbGeographyExt.PointFromCoordinates(23.45, -67.89),
            };
            this._anyTrail = new Trail
            {
                WtaId = "any-wta-trail-id",
                Name = "Any Trail Name",
                Url = new Uri("http://any/trail/uri"),
                Location = DbGeographyExt.PointFromCoordinates(12.34, -56.78),
            };

            this._routeServiceMock = new Mock<IRouteService>(MockBehavior.Strict);
            this._routeServiceMock
                .Setup(rs => rs.CalculateRouteAsync(It.IsAny<RouteRequest>()))
                .Returns(TaskExt.WrapInTask(() => this.CreateRouteResponse(123456)));

            Mock<IRouteServiceFactory> routeServiceFactoryMock = new Mock<IRouteServiceFactory>(MockBehavior.Strict);
            routeServiceFactoryMock
                .Setup(rsf => rsf.CreateRouteService())
                .Returns(this._routeServiceMock.Object);

            this._credentialsMock = new Mock<IBingMapsCredentials>(MockBehavior.Strict);
            this._credentialsMock
                .SetupGet(c => c.ApplicationId)
                .Returns("anyApplicationId");

            this._extender = new DrivingDistanceExtender
            {
                RouteServiceFactory = routeServiceFactoryMock.Object,
                BingMapsCredentials = this._credentialsMock.Object,
                Logger = new StubLog(),
            };
        }

        /// <summary>
        /// Clean up test helper objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.ClearDatabase();
        }

        /// <summary>
        /// Verify that <see cref="DrivingDistanceExtender.Extend"/> adds driving
        /// directions for address / trail pairs.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AddsDrivingDirections()
        {
            const int drivingTimeSeconds = 234567;

            // Arrange
            int trailId = this.RegisterTestData(this._anyTrail, this._anyAddress);

            this._routeServiceMock
                .Setup(rs => rs.CalculateRouteAsync(It.IsAny<RouteRequest>()))
                .Returns(TaskExt.WrapInTask(() => this.CreateRouteResponse(drivingTimeSeconds)))
                .Verifiable();

            // Act
            this.RunExtender(trailId);

            // Assert
            this._routeServiceMock.Verify();
            DrivingDirections directions = this.FindDrivingDirections(trailId, this._anyAddress);
            Assert.IsNotNull(directions);
            Assert.AreEqual(drivingTimeSeconds, directions.DrivingTimeSeconds);
        }

        /// <summary>
        /// Verify that <see cref="DrivingDistanceExtender.Extend"/> supplies credentials
        /// to the Bing Maps API.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void SendsCredentials()
        {
            const string anyApplicationId = "any application id";

            // Arrange
            this._credentialsMock
                .SetupGet(c => c.ApplicationId)
                .Returns(anyApplicationId)
                .Verifiable();

            bool hasAuthentication = false;
            this._routeServiceMock
                .Setup(rs => rs.CalculateRouteAsync(It.IsAny<RouteRequest>()))
                .Callback((RouteRequest rr) =>
                {
                    hasAuthentication = rr.Credentials != null &&
                        rr.Credentials.ApplicationId == anyApplicationId;
                })
                .Returns(TaskExt.WrapInTask(() => this.CreateRouteResponse(12345)));

            int trailId = this.RegisterTestData(this._anyTrail, this._anyAddress);

            // Act
            this.RunExtender(trailId);

            // Assert
            this._credentialsMock.Verify();
            Assert.IsTrue(hasAuthentication);
        }

        /// <summary>
        /// Verify that <see cref="DrivingDistanceExtender.Extend"/> skips
        /// trails that don't have a valid location.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void SkipTrailsWithoutLocation()
        {
            // Arrange
            this._anyTrail.Location = null;
            int trailId = this.RegisterTestData(this._anyTrail, this._anyAddress);

            // Act
            this.RunExtender(trailId);

            // Assert
            this._routeServiceMock.Verify(rs => rs.CalculateRouteAsync(It.IsAny<RouteRequest>()), Times.Never());
        }

        /// <summary>
        /// Verify that <see cref="DrivingDistanceExtender.Extend"/> skips
        /// trails that already have driving details.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void SkipsTrailsWithExistingDrivingDistance()
        {
            // Arrange
            this._anyAddress.Directions.Add(new DrivingDirections
            {
                Address = this._anyAddress,
                Trail = this._anyTrail,
                DrivingTimeSeconds = 12345,
            });
            int trailId = this.RegisterTestData(this._anyTrail, this._anyAddress);

            // Act
            this.RunExtender(trailId);

            // Assert
            this._routeServiceMock.Verify(rs => rs.CalculateRouteAsync(It.IsAny<RouteRequest>()), Times.Never());
        }

        /// <summary>
        /// Clear test data from the database.
        /// </summary>
        private void ClearDatabase()
        {
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                trailContext.ClearDatabase();
                trailContext.Addresses.Truncate();

                trailContext.SaveChanges();
            }
        }

        /// <summary>
        /// Create a <see cref="RouteRequest"/> with the given driving time.
        /// </summary>
        /// <param name="drivingTimeSeconds">The driving time in seconds.</param>
        /// <returns>A new <see cref="RouteRequest"/> instance.</returns>
        private RouteResponse CreateRouteResponse(int drivingTimeSeconds)
        {
            return new RouteResponse
            {
                Result = new RouteResult
                {
                    Summary = new RouteSummary
                    {
                        TimeInSeconds = drivingTimeSeconds,
                    },
                },
            };
        }

        /// <summary>
        /// Register test data with the datastore context.
        /// </summary>
        /// <param name="trail">Test trail to register.</param>
        /// <param name="addresses">The test addresses to register.</param>
        /// <returns>The ID of the trail in the datastore.</returns>
        private int RegisterTestData(Trail trail, params Address[] addresses)
        {
            int trailId;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.Trails.Add(trail);
                foreach (Address address in addresses)
                {
                    context.Addresses.Add(address);
                }

                context.SaveChanges();
                trailId = trail.Id;
            }

            return trailId;
        }

        /// <summary>
        /// Run <see cref="DrivingDistanceExtender.Extend"/> and save database context changes.
        /// </summary>
        /// <param name="trailId">The ID of the trail to add driving directions for.</param>
        private void RunExtender(int trailId)
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                Trail trail = context.Trails.Find(trailId);
                this._extender.Extend(trail, context).Wait();

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Find driving directions associated with the given trail ID and address.
        /// </summary>
        /// <param name="trailId">The ID of the trail in the data store.</param>
        /// <param name="address">The address to look up.</param>
        /// <returns>The associated driving directions, or null if none exist.</returns>
        private DrivingDirections FindDrivingDirections(int trailId, Address address)
        {
            DbGeographyPointComparer comparer = new DbGeographyPointComparer();

            DrivingDirections directions;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                Address innerAddress = context.Addresses
                    .ToList()
                    .Where(a => comparer.Equals(a.Coordinate, address.Coordinate))
                    .FirstOrDefault();

                directions = innerAddress == null ?
                    null :
                    innerAddress.Directions
                        .Where(d => d.TrailId == trailId)
                        .FirstOrDefault();
            }

            return directions;
        }
    }
}
