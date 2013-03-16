namespace MyTrails.Importer.Test.Extenders
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Extenders;
    using MyTrails.Importer.Test.Logging;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Unit tests for the <see cref="TripReportExtender"/> class.
    /// </summary>
    [TestClass]
    public class TripReportExtenderTests
    {
        /// <summary>
        /// Sample trip report ID to use during testing.
        /// </summary>
        private const string AnyWtaTripReportId = "any-wta-trip-report-id";

        /// <summary>
        /// Sample <see cref="Trail"/> to use during testing.
        /// </summary>
        private static readonly Trail AnyTrail = new Trail
        {
            Name = "Any Trail Name",
            WtaId = "any-wta-id",
            Url = new Uri("http://any/trail/uri"),
        };

        /// <summary>
        /// Sample <see cref="TripType"/> ID to use during testing.
        /// </summary>
        private string _anyTripTypeId;

        /// <summary>
        /// The <see cref="TripReportExtender"/> instance to test against.
        /// </summary>
        private TripReportExtender _extender;

        /// <summary>
        /// Mock <see cref="IWtaClient"/> to inject test behaviors.
        /// </summary>
        private Mock<IWtaClient> _wtaClientMock;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.ClearDatabase();
                context.SaveChanges();

                this._anyTripTypeId = context.TripTypes.First().WtaId;
            }

            this._wtaClientMock = new Mock<IWtaClient>(MockBehavior.Strict);
            this._wtaClientMock
                .Setup(wc => wc.FetchTripReports(It.IsAny<string>()))
                .Returns((string ti) => TaskExt.WrapInTask<IList<WtaTripReport>>(() => new List<WtaTripReport>
                {
                    new WtaTripReport
                    {
                        Title = "Any title",
                        Author = "Any author",
                        Date = DateTime.Now,
                        FullReportUrl = new Uri(new Uri("http://any.base.url"), AnyWtaTripReportId),
                        HikeType = this._anyTripTypeId,
                    }
                }));

            this._extender = new TripReportExtender
            {
                WtaClient = this._wtaClientMock.Object,
                Logger = new StubLog(),
            };
        }

        /// <summary>
        /// Clean up test data.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.ClearDatabase();
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Verify that <see cref="TripReportExtender.Extends"/> adds trip reports for the trail.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void AddsTripReport()
        {
            // Arrange
            int trailId = this.AddTestData(AnyTrail);

            // Act
            this.RunExtender(trailId).Wait();

            // Assert
            TripReport report;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                report = context.TripReports
                    .Where(tr => tr.Trails.Any(t => t.Id == trailId))
                    .FirstOrDefault();
            }

            Assert.IsNotNull(report);
        }

        /// <summary>
        /// Add trail data to the datastore and return the trail id.
        /// </summary>
        /// <param name="trail">The trail to add to the datastore.</param>
        /// <returns>The ID of the added trail.</returns>
        private int AddTestData(Trail trail)
        {
            int id;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.Trails.Add(trail);
                context.SaveChanges();

                id = trail.Id;
            }

            return id;
        }

        /// <summary>
        /// Run the extender for the given trail ID.
        /// </summary>
        /// <param name="trailId">The ID of the trail to run for.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task RunExtender(int trailId)
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                Trail trail = context.Trails.Find(trailId);
                await this._extender.Extend(trail, context);

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    Assert.Fail("Datastore save failed with validation error: {0}", ex);
                }
            }
        }
    }
}
