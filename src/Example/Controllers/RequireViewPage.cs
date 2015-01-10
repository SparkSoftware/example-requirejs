using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Spark.Example.MultiPageMvcWithRequireJs.Controllers
{
    /// <summary>
    /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
    /// </summary>
    public abstract class RequireViewPage : RequireViewPage<Object>
    { }

    /// <summary>
    /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
    /// </summary>
    public abstract class RequireViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// The requirejs module id associated with this view page (i.e., 'view/{controller}/{action}/main')
        /// </summary>
        public HtmlString MainModule
        {
            get
            {
                var routeData = ViewContext.RouteData;
                var area = routeData.Values["Area"] as String;
                var action = routeData.Values["Action"] as String;
                var controller = routeData.Values["Controller"] as String;
                var moduleName = String.IsNullOrWhiteSpace(action) || String.IsNullOrWhiteSpace(controller) ? null : RequireModule.GetModuleName(Server.MapPath(Url.GetScriptBase()), controller, action, area);

                return new HtmlString(moduleName);
            }
        }

        /// <summary>
        /// Get the relative module name from the main view model.
        /// </summary>
        /// <param name="relativeName">The relative module name.</param>
        public HtmlString Module(String relativeName)
        {
            return new HtmlString(VirtualPathUtility.Combine("/" + MainModule, relativeName).Substring(1));
        }

        /// <summary>
        /// The requirejs module configuration associated with this view page.
        /// </summary>
        /// <remarks>
        /// Ensure @RenderModuleConfig added prior to `require.js` script include.
        /// </remarks>
        public HtmlString RenderModuleConfig()
        {
            var tag = new TagBuilder("script");
            var config = new { baseUrl = Url.Content("~/"), consoleEnabled = Request.IsLocal || Context.IsDebuggingEnabled };

            tag.Attributes.Add("type", "text/javascript");
            tag.InnerHtml = "var require = " + Json.Encode(new { baseUrl = Url.GetScriptBase(), config = new { config } });

            return new HtmlString(tag.ToString());
        }
    }
}