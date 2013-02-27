namespace MyTrails.Importer.Composition
{
    using System.ComponentModel.Composition;

    using CommandLine;

    /// <summary>
    /// MEF exports to make available for composition.
    /// </summary>
    public static class Exports
    {
        /// <summary>
        /// Exported <see cref="ICommandLineParser"/> instance.
        /// </summary>
        [Export(typeof(ICommandLineParser))]
        public static ICommandLineParser CommandLineParser
        {
            get { return CommandLine.CommandLineParser.Default; }
        }
    }
}
