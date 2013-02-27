namespace MyTrails.Importer.Wta
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for communicating with wta.org
    /// </summary>
    public interface IWtaClient
    {
        /// <summary>
        /// Fetch trail definitions from WTA.
        /// </summary>
        /// <returns>A sequence of trails from WTA.</returns>
        Task<IList<WtaTrail>> FetchTrails();
    }
}
