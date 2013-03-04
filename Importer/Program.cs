namespace MyTrails.Importer
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Threading.Tasks;
    using CommandLine;
    using log4net;

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
        /// Parse commandline options and execute the trails importer.
        /// </summary>
        /// <param name="options">Execution options.</param>
        public void Run(ExecutionOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.Logger.Info("Beginning execution.");

            this.TrailsImporter.Modes = options.Modes;
            const string errorStringFormat = "Errors encountered during execution:\n{0}";
            try
            {
                Task t = this.TrailsImporter.Run();
                t.Wait();
            }
            catch (AggregateException ae)
            {
                this.Logger.ErrorFormat(errorStringFormat, string.Join(Environment.NewLine, ae.Flatten().InnerExceptions));
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.ErrorFormat(errorStringFormat, ex);
                throw;
            }
            finally
            {
                this.Logger.Info("Done!");
            }
        }

        /// <summary>
        /// Entry point to the application.
        /// </summary>
        /// <param name="args">Commandline arguments.</param>
        /// <returns>0 on success, or non-zero otherwise.</returns>
        internal static int Main(string[] args)
        {
            int returnCode;

            try
            {
                ExecutionOptions options = ParseCommandLine(args);

                using (ApplicationCatalog catalog = BuildCompositionCatalog())
                using (CompositionContainer container = new CompositionContainer(catalog))
                {
                    Program p = container.GetExportedValue<Program>();
                    p.Run(options);
                }

                returnCode = 0;
            }
            catch
            {
                returnCode = -1;
            }

            return returnCode;
        }

        /// <summary>
        /// Parse execution options from the command line.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Parsed commandline options.</returns>
        private static ExecutionOptions ParseCommandLine(string[] args)
        {
            bool parserSuccess;
            ExecutionOptions options = new ExecutionOptions();
            using (Parser parser = new Parser())
            {
                parserSuccess = parser.ParseArguments(args, options);
            }

            if (!parserSuccess)
            {
                throw new InvalidOperationException("Error parsing command line arguments.");
            }

            return options;
        }
    }
}
