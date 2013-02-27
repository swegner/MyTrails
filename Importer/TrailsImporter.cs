namespace MyTrails.Importer
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

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
        /// Import and update trails according to the configured <see cref="Modes"/>
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailsImporter.Run"/>
        public async Task Run()
        {
        }
    }
}
