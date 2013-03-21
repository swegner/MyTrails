namespace MyTrails.ServiceLib
{
    using System.Threading.Tasks;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    public interface ITrailsImporter
    {
        /// <summary>
        /// Import and update trails.
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        Task Run();
    }
}
