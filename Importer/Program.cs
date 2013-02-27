namespace MyTrails.Importer
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;
    using CommandLine;
    using CommandLine.Text;
    using log4net;

    /// <summary>
    /// Pulls new and updated trail information from the WTA website.
    /// </summary>
    [Export]
    public class Program
    {
        /// <summary>
        /// The command line parser.
        /// </summary>
        [Import]
        public ICommandLineParser CommandLineParser { get; set; }

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
        /// Parse commandline options and execute the trails importer.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public void Run(string[] args)
        {
            this.Logger.Debug("Beginning execution.");

            Options options = new Options();
            if (this.CommandLineParser.ParseArguments(args, options))
            {
                this.TrailsImporter.Modes = options.Modes;
                Task t = this.TrailsImporter.Run();

                t.Wait();
            }
        }

        /// <summary>
        /// Entry point to the application.
        /// </summary>
        /// <param name="args">Commandline arguments.</param>
        internal static void Main(string[] args)
        {
            using (ApplicationCatalog catalog = new ApplicationCatalog())
            using (CompositionContainer container = new CompositionContainer(catalog))
            {
                Program p = container.GetExportedValue<Program>();
                p.Run(args);
            }
        }

        /// <summary>
        /// Commandline options.
        /// </summary>
        private class Options : CommandLineOptionsBase
        {
            /// <summary>
            /// The import mode.
            /// </summary>
            [Option(shortName: "m", longName: "mode", DefaultValue = ImportModes.ImportAndUpdate,
                HelpText = "Whether to import new trails, update existing trails, or both")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "Method called via reflection.")]
            public ImportModes Modes { get; set; }

            /// <summary>
            /// Create a usage string to display if commandline parsing fails.
            /// </summary>
            /// <returns>A command-line usage string.</returns>
            [HelpOption]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "Method called via reflection.")] 
            public string Usage()
            {
                HelpText helpText = new HelpText
                {
                    Heading = new HeadingInfo("MyTrails Importer",
                        Assembly.GetEntryAssembly().GetName().Version.ToString()),
                };
                helpText.AddOptions(this);

                return helpText;
            }
        }
    }
}
