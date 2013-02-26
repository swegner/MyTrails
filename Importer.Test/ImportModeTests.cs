namespace Importer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MyTrails.Importer;

    /// <summary>
    /// Unit tests for the <see cref="ImportMode"/> enum.
    /// </summary>
    [TestClass]
    public class ImportModeTests
    {
        /// <summary>
        /// Verify that the <see cref="ImportMode"/> default value is <see cref="ImportMode.None"/>
        /// </summary>
        [TestMethod]
        public void DefaultValueIsNone()
        {
            // Act
            const ImportMode importMode = default(ImportMode);

            // Assert
            Assert.AreEqual(ImportMode.None, importMode);
        }

        /// <summary>
        /// Verify that each singular flag mode is mutually exclusive.
        /// </summary>
        [TestMethod]
        public void VerifySingularModes()
        {
            // Arrange
            ImportMode[] singularFlagModes = new[]
            {
                ImportMode.ImportOnly,
                ImportMode.UpdateOnly,      
            };

            // Act / Assert
            for (int i = 0; i < singularFlagModes.Length; i++)
            {
                ImportMode modeA = singularFlagModes[i];
                Assert.AreNotEqual(0, modeA, "ImportMode {0} equal to 0.", modeA);

                for (int j = i + 1; j < singularFlagModes.Length; j++)
                {
                    ImportMode modeB = singularFlagModes[j];
                    Assert.AreEqual(modeA | modeB, modeA ^ modeB, "ImportModes {0} and {1} are not mutually exclusive.");
                }
            }
        }

        /// <summary>
        /// Verify that <see cref="ImportMode.ImportAndUpdate"/> is the combination of <see cref="ImportMode.ImportOnly"/> and
        /// <see cref="ImportMode.UpdateOnly"/>
        /// </summary>
        [TestMethod]
        public void VerifyImportAndUpdate()
        {
            Assert.AreEqual(ImportMode.ImportOnly | ImportMode.UpdateOnly, ImportMode.ImportAndUpdate);
        }
    }
}
