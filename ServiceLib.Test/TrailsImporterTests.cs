namespace MyTrails.ServiceLib.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.ServiceLib.Extenders;
    using MyTrails.ServiceLib.Test.Logging;
    using MyTrails.ServiceLib.Test.Retry;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Unit tests for the <see cref="TrailsImporter"/> class.
    /// </summary>
    [TestClass]
    public class TrailsImporterTests : IDisposable
    {
        /// <summary>
        /// Existing trails to seed into the <see cref="MyTrailsContext.Trails"/> datastore.
        /// </summary>
        private static readonly Trail[] ExistingTrails = new[]
        {
            new Trail
            {
                Name = "Existing Trail 1 ", 
                WtaId = "existing-trail-1",
                Url = new Uri("http://existing/trail/1")
            },
        };

        /// <summary>
        /// Additional trials to discover during import.
        /// </summary>
        private static readonly WtaTrail[] NewTrails = new[]
        {
            new WtaTrail
            {
                Title = "New Trail 1 ",
                Uid = "new-trail-1",
                Url = new Uri("http://new/trail/1")
            },
        };

        /// <summary>
        /// List of trails to return when importing new trails.
        /// </summary>
        private static readonly IList<WtaTrail> TrailsToImport = ExistingTrails
            .Select(t => new WtaTrail
            {
                Title = t.Name,
                Url = t.Url,
                Uid = t.WtaId,
            })
            .Concat(NewTrails)
            .ToList();

        /// <summary>
        /// The importer instance to test against.
        /// </summary>
        private TrailsImporter _importer;

        /// <summary>
        /// Database connection context.
        /// </summary>
        private MyTrailsContext _dataContext;

        /// <summary>
        /// Mock <see cref="IWtaClient"/> to inject test behavior.
        /// </summary>
        private Mock<IWtaClient> _wtaClientMock;

        /// <summary>
        /// Mock <see cref="ITrailFactory"/> to inject test behavior.
        /// </summary>
        private Mock<ITrailFactory> _trailFactoryMock;

        /// <summary>
        /// Mock <see cref="ITrailExtender"/> to inject test behavior.
        /// </summary>
        private Mock<ITrailExtender> _trailExtenderMock;

        /// <summary>
        /// Whether the instance has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.InitializeDatabase();
            this.InitializeMocks();

            Mock<IImporterConfiguration> configurationMock = new Mock<IImporterConfiguration>(MockBehavior.Strict);
            configurationMock
                .SetupGet(c => c.HeartbeatInterval)
                .Returns(TimeSpan.FromMilliseconds(1234));

            this._importer = new TrailsImporter
            {
                WtaClient = this._wtaClientMock.Object,
                TrailFactory = this._trailFactoryMock.Object,
                TrailExtenders =
                {
                    this._trailExtenderMock.Object,
                },
                Configuration = configurationMock.Object,
                Logger = new StubLog(),
            };
        }

        /// <summary>
        /// Clean up test resources.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this._dataContext.ClearDatabase();
            this._dataContext.SaveChanges();

            this.Dispose();
        }

        /// <summary>
        /// Verify that <see cref="TrailsImporter.Run"/> does not throw an exception.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void RunDoesNotThrow()
        {
            // Act
            Task t = this._importer.Run();
            t.Wait();
        }

        /// <summary>
        /// Verify that new hikes are fetched via <see cref="TrailsImporter.WtaClient"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void FetchesFromWtaClient()
        {
            // Arrange
            this._wtaClientMock
                .Setup(wc => wc.FetchTrails())
                .Returns(() => TaskExt.WrapInTask(() => TrailsToImport))
                .Verifiable();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._wtaClientMock.Verify();
        }

        /// <summary>
        /// Verify that <see cref="Trail"/> instances are created for imported <see cref="WtaTrail"/>s
        /// using the <see cref="TrailsImporter.TrailFactory"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void CreatesTrailsUsingFactory()
        {
            // Arrange
            this._trailFactoryMock
            .Setup(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<MyTrailsContext>()))
            .Returns((WtaTrail wt, MyTrailsContext c) => 
                new Trail
                {
                    Name = wt.Title,
                    WtaId = wt.Uid,
                    Url = wt.Url,
                })
            .Verifiable();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._trailFactoryMock.Verify();
        }

        /// <summary>
        /// Verify that existing trails in the database are updated.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ExistingTrailsUpdated()
        {
            // Arrange
            this._trailFactoryMock
                .Setup(tf => tf.UpdateTrail(It.IsAny<Trail>(), It.IsAny<WtaTrail>(), It.IsAny<MyTrailsContext>()))
                .Verifiable();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._trailFactoryMock.Verify();
        }

        /// <summary>
        /// Verify that duplicates encountered during import are removed.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ImportSkipsDuplicates()
        {
            // Arrange
            IList<WtaTrail> dupedTrails = TrailsToImport
                .Concat(TrailsToImport)
                .ToList();
            this._wtaClientMock
                .Setup(wc => wc.FetchTrails())
                .Returns(() => TaskExt.WrapInTask(() => dupedTrails));

            // Act
            this._importer.Run().Wait();

            // Assert
            this._trailFactoryMock.Verify(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<MyTrailsContext>()),
                Times.Exactly(NewTrails.Length));
        }

        /// <summary>
        /// Verify that newly imported trails are added to the database.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ImportedTrailsAddedToDatabase()
        {
            // Act
            this._importer.Run().Wait();

            // Assert
            string expectedId = NewTrails.First().Uid;
            Assert.IsTrue(this._dataContext.Trails.Any(t => t.WtaId == expectedId));
        }

        /// <summary>
        /// Verify that each extender is executed for newly imported trails.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ImportCallsExtenders()
        {
            // Arrange
            this._trailExtenderMock
                .Setup(te => te.Extend(It.IsAny<Trail>(), It.IsAny<MyTrailsContext>()))
                .Returns(TaskExt.CreateNopOpTask)
                .Verifiable();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._trailExtenderMock.Verify();
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <seealso cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <param name="disposing">Whether it is safe to reference managed objects.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._dataContext != null)
                    {
                        this._dataContext.Dispose();
                        this._dataContext = null;
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Initialize test mock objects.
        /// </summary>
        private void InitializeMocks()
        {
            this._wtaClientMock = new Mock<IWtaClient>(MockBehavior.Strict);
            this._wtaClientMock
                .Setup(wc => wc.FetchTrails())
                .Returns(() => TaskExt.WrapInTask(() => TrailsToImport));
            this._wtaClientMock
                .Setup(wc => wc.BuildRetryPolicy(It.IsAny<ILog>()))
                .Returns(new RetryPolicy(new StubErrorDetectionStrategy(), retryCount: 0));

            this._trailFactoryMock = new Mock<ITrailFactory>(MockBehavior.Strict);
            this._trailFactoryMock
                .Setup(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<MyTrailsContext>()))
                .Returns((WtaTrail wt, MyTrailsContext c) => 
                    new Trail
                    {
                        Name = wt.Title,
                        WtaId = wt.Uid,
                        Url = wt.Url,
                    });
            this._trailFactoryMock
                .Setup(tf => tf.UpdateTrail(It.IsAny<Trail>(), It.IsAny<WtaTrail>(), It.IsAny<MyTrailsContext>()));

            this._trailExtenderMock = new Mock<ITrailExtender>(MockBehavior.Strict);
            this._trailExtenderMock
                .Setup(te => te.Extend(It.IsAny<Trail>(), It.IsAny<MyTrailsContext>()))
                .Returns(TaskExt.CreateNopOpTask);
        }

        /// <summary>
        /// Initialize the database context and seed contents.
        /// </summary>
        private void InitializeDatabase()
        {
            this._dataContext = new MyTrailsContext();

            // Clear any existing contents.
            this._dataContext.ClearDatabase();

            // Seed test data
            foreach (Trail existingTrail in ExistingTrails)
            {
                this._dataContext.Trails.Add(existingTrail);
            }

            this._dataContext.SaveChanges();
        }
    }
}
