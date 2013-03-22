namespace MyTrails.Service
{
    using System.ComponentModel.Composition.Hosting;
    using System.Threading.Tasks;
    using System.Web.Http;
    using MyTrails.ServiceLib;

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
            using (ApplicationCatalog catalog = new ApplicationCatalog())
            using (CompositionContainer container = new CompositionContainer(catalog))
            {
                ITrailsImporter importer = container.GetExportedValue<ITrailsImporter>();
                await importer.Run();
            }
        }
    }
}