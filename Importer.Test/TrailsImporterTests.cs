namespace Importer.Test
{
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Importer;

    /// <summary>
    /// Unit tests for the <see cref="TrailsImporter"/> class.
    /// </summary>
    [TestClass]
    public class TrailsImporterTests
    {
        /// <summary>
        /// The importer instance to test against.
        /// </summary>
        private TrailsImporter _importer;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._importer = new TrailsImporter();
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
    }
}
