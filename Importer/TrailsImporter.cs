namespace MyTrails.Importer
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using MyTrails.Importer.Wta;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    [Export(typeof(ITrailsImporter))]
    public class TrailsImporter : ITrailsImporter
    {
        /// <summary>
        /// The run mode for the importer.
        /// </summary>
        /// <seealso cref="ITrailsImporter.Modes"/>
        public ImportModes Modes { get; set; }

        /// <summary>
        /// Interface for communicating with WTA.
        /// </summary>
        [Import]
        public IWtaClient WtaClient { get; set; }

        /// <summary>
        /// Import and update trails according to the configured <see cref="Modes"/>
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailsImporter.Run"/>
        public async Task Run()
        {
            ImportModes modes = this.Modes;
            if (!Enum.IsDefined(typeof(ImportModes), modes) || modes == ImportModes.None)
            {
                throw new InvalidOperationException(string.Format("Invalid ImportModes specified: {0}", modes));
            }

            if (modes.HasFlag(ImportModes.ImportOnly))
            {
                await this.WtaClient.FetchTrails();
            }
        }
    }
}
