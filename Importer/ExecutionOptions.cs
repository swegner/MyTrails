namespace MyTrails.Importer
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Commandline options.
    /// </summary>
    public class ExecutionOptions
    {
        /// <summary>
        /// The import mode.
        /// </summary>
        [Option(shortName: 'm', longName: "mode", DefaultValue = ImportModes.ImportAndUpdate,
            HelpText = "Whether to import new trails, update existing trails, or both")]
        public ImportModes Modes { get; set; }

        /// <summary>
        /// Create a usage string to display if commandline parsing fails.
        /// </summary>
        /// <returns>A command-line usage string.</returns>
        [HelpOption]
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