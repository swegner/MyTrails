namespace MyTrails.Importer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    public interface ITrailsImporter
    {
        /// <summary>
        /// The run mode for the importer.
        /// </summary>
        ImportMode Mode { get; set; }

        /// <summary>
        /// Import and update trails according to the configured <see cref="Mode"/>
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        Task Run();
    }
}
