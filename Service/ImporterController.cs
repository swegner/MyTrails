namespace MyTrails.Service
{
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// MyTrails importer interface.
    /// </summary>
    public class ImporterController : ApiController
    {
        /// <summary>
        /// Import and update trails.
        /// </summary>
        /// <returns>Task for asyncrhonous completion.</returns>
        public async Task Get()
        {
            await Task.Yield();
        }
    }
}