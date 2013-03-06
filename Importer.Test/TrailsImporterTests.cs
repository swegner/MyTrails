namespace MyTrails.Importer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer;
    using MyTrails.Importer.Test.Logging;
    using MyTrails.Importer.Wta;

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
            .Select(t => new WtaTrail { Uid = t.WtaId })
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

            this._importer = new TrailsImporter
            {
                Modes = ImportModes.ImportAndUpdate,
                WtaClient = this._wtaClientMock.Object,
                TrailFactory = this._trailFactoryMock.Object,
                Logger = new StubLog(),
            };
        }

        /// <summary>
        /// Clean up test resources.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this._dataContext.Trails.Truncate();
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
        /// Verify that <see cref="TrailsImporter.Run"/> validates <see cref="TrailsImporter.Modes"/>
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ValidatesModes()
        {
            // Arrange
            this._importer.Modes = ImportModes.None;
            Action act = () => this._importer.Run().Wait();

            // Act / Assert
            act.ShouldThrow<Exception>();
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
        /// Verify that <see cref="ImportModes.UpdateOnly"/> skips importing.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void UpdateOnlySkipsImport()
        {
            // Arrange
            this._importer.Modes = ImportModes.UpdateOnly;

            // Act
            this._importer.Run().Wait();

            // Assert
            this._wtaClientMock.Verify(wc => wc.FetchTrails(), Times.Never());
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
            .Setup(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<IEnumerable<Region>>(), 
                It.IsAny<IEnumerable<Guidebook>>(), It.IsAny<IEnumerable<RequiredPass>>(), 
                It.IsAny<IEnumerable<TrailFeature>>(), It.IsAny<IEnumerable<TrailCharacteristic>>()))
            .Returns((WtaTrail wt, IEnumerable<Region> rs, IEnumerable<Guidebook> gbs, 
                IEnumerable<RequiredPass> ps, IEnumerable<TrailFeature> tfs, IEnumerable<TrailCharacteristic> tcs) => 
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
        /// Verify that existing trails in the database are deduped before sending them through
        /// the <see cref="TrailsImporter.TrailFactory"/>.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Unit)]
        public void ExistingTrailsDeduped()
        {
            // Arrange
            WtaTrail existingTrail = ExistingTrails
                .Join(TrailsToImport, t => t.WtaId, wt => wt.Uid, (t, wt) => wt)
                .First();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._trailFactoryMock.Verify(tf => tf.CreateTrail(existingTrail, It.IsAny<IEnumerable<Region>>(),
                It.IsAny<IEnumerable<Guidebook>>(), It.IsAny<IEnumerable<RequiredPass>>(),
                It.IsAny<IEnumerable<TrailFeature>>(), It.IsAny<IEnumerable<TrailCharacteristic>>()),
                Times.Never());
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
            this._trailFactoryMock.Verify(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<IEnumerable<Region>>(),
                It.IsAny<IEnumerable<Guidebook>>(), It.IsAny<IEnumerable<RequiredPass>>(),
                It.IsAny<IEnumerable<TrailFeature>>(), It.IsAny<IEnumerable<TrailCharacteristic>>()),
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

            this._trailFactoryMock = new Mock<ITrailFactory>(MockBehavior.Strict);
            this._trailFactoryMock
                .Setup(tf => tf.CreateTrail(It.IsAny<WtaTrail>(), It.IsAny<IEnumerable<Region>>(),
                    It.IsAny<IEnumerable<Guidebook>>(), It.IsAny<IEnumerable<RequiredPass>>(),
                    It.IsAny<IEnumerable<TrailFeature>>(), It.IsAny<IEnumerable<TrailCharacteristic>>()))
                .Returns((WtaTrail wt, IEnumerable<Region> rs, IEnumerable<Guidebook> gs, 
                    IEnumerable<RequiredPass> rps, IEnumerable<TrailFeature> tfs, IEnumerable<TrailCharacteristic> tcs) => 
                    new Trail
                    {
                        Name = wt.Title,
                        WtaId = wt.Uid,
                        Url = wt.Url,
                    });
        }

        /// <summary>
        /// Initialize the database context and seed contents.
        /// </summary>
        private void InitializeDatabase()
        {
            this._dataContext = new MyTrailsContext();

            // Clear any existing contents.
            this._dataContext.Trails.Truncate();

            // Seed test data
            foreach (Trail existingTrail in ExistingTrails)
            {
                this._dataContext.Trails.Add(existingTrail);
            }

            this._dataContext.SaveChanges();
        }
    }
}
