namespace MyTrails.Importer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyTrails.Importer;
    using MyTrails.Importer.Test.Logging;

    /// <summary>
    /// Unit test for the <see cref="Program"/> class.
    /// </summary>
    [TestClass]
    public class ProgramTests
    {
        /// <summary>
        /// The <see cref="Program"/> instance to test against.
        /// </summary>
        private Program _program;

        /// <summary>
        /// Mock <see cref="ITrailsImporter"/> used to inject test behavior.
        /// </summary>
        private Mock<ITrailsImporter> _importerMock;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._importerMock = new Mock<ITrailsImporter>(MockBehavior.Strict);
            this._importerMock.SetupProperty(i => i.Modes);
            this._importerMock
                .Setup(i => i.Run())
                .Returns(TaskExt.CreateNopOpTask);

            this._program = new Program
            {
                TrailsImporter = this._importerMock.Object,
                Logger = new StubLog(),
            };
        }

        /// <summary>
        /// Verify that <see cref="ITrailsImporter.Modes"/> is set from parsed commandline arguments.
        /// </summary>
        [TestMethod]
        public void ModesSetFromCommandLine()
        {
            const ImportModes anyModes = ImportModes.UpdateOnly;
           
            // Arrange
            ExecutionOptions options = new ExecutionOptions { Modes = anyModes };

            // Act
            this._program.Run(options);

            // Assert
            this._importerMock.VerifySet(i => i.Modes = anyModes);
        }
    }
}
