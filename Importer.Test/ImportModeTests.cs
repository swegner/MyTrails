namespace MyTrails.Importer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Importer;

    /// <summary>
    /// Unit tests for the <see cref="ImportModes"/> enum.
    /// </summary>
    [TestClass]
    public class ImportModeTests
    {
        /// <summary>
        /// Verify that the <see cref="ImportModes"/> default value is <see cref="ImportModes.None"/>
        /// </summary>
        [TestMethod]
        public void DefaultValueIsNone()
        {
            // Act
            const ImportModes importMode = default(ImportModes);

            // Assert
            Assert.AreEqual(ImportModes.None, importMode);
        }

        /// <summary>
        /// Verify that each singular flag mode is mutually exclusive.
        /// </summary>
        [TestMethod]
        public void VerifySingularModes()
        {
            // Arrange
            ImportModes[] singularFlagModes = new[]
            {
                ImportModes.ImportOnly,
                ImportModes.UpdateOnly,      
            };

            // Act / Assert
            for (int i = 0; i < singularFlagModes.Length; i++)
            {
                ImportModes modeA = singularFlagModes[i];
                Assert.AreNotEqual(0, modeA, "ImportModes {0} equal to 0.", modeA);

                for (int j = i + 1; j < singularFlagModes.Length; j++)
                {
                    ImportModes modeB = singularFlagModes[j];
                    Assert.AreEqual(modeA | modeB, modeA ^ modeB, "ImportModes {0} and {1} are not mutually exclusive.");
                }
            }
        }

        /// <summary>
        /// Verify that <see cref="ImportModes.ImportAndUpdate"/> is the combination of <see cref="ImportModes.ImportOnly"/> and
        /// <see cref="ImportModes.UpdateOnly"/>
        /// </summary>
        [TestMethod]
        public void VerifyImportAndUpdate()
        {
            Assert.AreEqual(ImportModes.ImportOnly | ImportModes.UpdateOnly, ImportModes.ImportAndUpdate);
        }
    }
}
