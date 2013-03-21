namespace MyTrails.Importer
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Threading.Tasks;
    using log4net;
    using MyTrails.ServiceLib;

    /// <summary>
    /// Pulls new and updated trail information from the WTA website.
    /// </summary>
    [Export]
    public class Program
    {
        /// <summary>
        /// Imports trails from WTA.
        /// </summary>
        [Import]
        public ITrailsImporter TrailsImporter { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Build the composition catalog for the application.
        /// </summary>
        /// <returns>The composition catalog for the application.</returns>
        public static ApplicationCatalog BuildCompositionCatalog()
        {
            return new ApplicationCatalog();
        }

        /// <summary>
        /// Entry point to the application.
        /// </summary>
        /// <returns>0 on success, or non-zero otherwise.</returns>
        internal static int Main()
        {
            int returnCode;

            try
            {
                using (ApplicationCatalog catalog = BuildCompositionCatalog())
                using (CompositionContainer container = new CompositionContainer(catalog))
                {
                    ITrailsImporter importer = container.GetExportedValue<ITrailsImporter>();
                    importer.Run().Wait();
                }

                returnCode = 0;
            }
            catch
            {
                returnCode = -1;
            }

            return returnCode;
        }
    }
}
