using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Spark.Example.MultiPageMvcWithRequireJs
{
    /// <summary>
    /// Defines the methods, properties, and events that are common to all application objects in an ASP.NET application.
    /// </summary>
    public class Application : HttpApplication
    {
        /// <summary>
        /// The current web client version.
        /// </summary>
        public static readonly Version Version = typeof(Application).Assembly.GetName().Version;

        /// <summary>
        /// Initialize a new <see cref="HttpApplication"/> instance.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Registers the controller action routes.
        /// </summary>
        /// <param name="routes">The global route configuration.</param>
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
