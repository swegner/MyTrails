namespace Importer.Test
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Run test assembly initilization and cleanup.
    /// </summary>
    [TestClass]
    public static class TestAssembly
    {
        /// <summary>
        /// Initialize test context for the assembly.
        /// </summary>
        /// <param name="context">The parameter is not used.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // DataDirectory must be set for localdb context.
            AppDomain.CurrentDomain.SetData("DataDirectory",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Empty));
        }
    }
}
