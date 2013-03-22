namespace MyTrails.Service
{
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Global web configuration.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Entry point for global configuration.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}");
        }
    }
}