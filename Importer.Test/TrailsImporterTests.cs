namespace Importer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Importer;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Unit tests for the <see cref="TrailsImporter"/> class.
    /// </summary>
    [TestClass]
    public class TrailsImporterTests
    {
        /// <summary>
        /// Sample <see cref="WtaTrail"/> to use during testing.
        /// </summary>
        private static readonly WtaTrail AnyWtaTrail = new WtaTrail();

        /// <summary>
        /// The importer instance to test against.
        /// </summary>
        private TrailsImporter _importer;

        /// <summary>
        /// Mock <see cref="IWtaClient"/> to inject test behavior.
        /// </summary>
        private Mock<IWtaClient> _wtaClientMock;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._wtaClientMock = new Mock<IWtaClient>(MockBehavior.Strict);
            this._wtaClientMock
                .Setup(wc => wc.FetchTrails())
                .Returns(WrapInTask((IEnumerable<WtaTrail>)new[] { AnyWtaTrail }));

            this._importer = new TrailsImporter
            {
                Modes = ImportModes.ImportAndUpdate,
                WtaClient = this._wtaClientMock.Object,
            };
        }

        /// <summary>
        /// Verify that <see cref="TrailsImporter.Run"/> does not throw an exception.
        /// </summary>
        [TestMethod]
        public void RunDoesNotThrow()
        {
            // Act
            Task t = this._importer.Run();
            t.Wait();
        }

        /// <summary>
        /// Verify that <see cref="TrailsImporter.Run"/> validates <see cref="TrailsImporter.Modes"/>
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public void FetchesFromWtaClient()
        {
            // Arrange
            this._wtaClientMock
                .Setup(wc => wc.FetchTrails())
                .Returns(WrapInTask((IEnumerable<WtaTrail>)new[] { AnyWtaTrail }))
                .Verifiable();

            // Act
            this._importer.Run().Wait();

            // Assert
            this._wtaClientMock.Verify();
        }

        /// <summary>
        /// Verify that <see cref="ImportModes.UpdateOnly"/> skips importing.
        /// </summary>
        [TestMethod]
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
        /// Wrap a result object in an instantaneous task.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="result">The result object to wrap.</param>
        /// <returns>An instantaneous task.</returns>
        private static Task<T> WrapInTask<T>(T result)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);

            return tcs.Task;
        }
    }
}
