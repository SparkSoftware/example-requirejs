using System;
using System.Web.Mvc;

namespace Spark.Example.MultiPageMvcWithRequireJs.Controllers
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/>.
    /// </summary>
    public static class UrlExtensions
    {
        private static readonly String ReleaseContent = "Content-" + Application.Version;
        private static readonly String DebugContent = "Content";

        /// <summary>
        /// Get the script folder's base url.
        /// </summary>
        /// <param name="url">The <see cref="UrlHelper"/> instance to extend.</param>
        public static String GetScriptBase(this UrlHelper url)
        {
            return url.Content("~/" + (url.RequestContext.HttpContext.IsDebuggingEnabled ? DebugContent : ReleaseContent) + "/js/");
        }

        /// <summary>
        /// Get the script url based on the specified <paramref name="relativePath"/>.
        /// </summary>
        /// <param name="url">The <see cref="UrlHelper"/> instance to extend.</param>
        /// <param name="relativePath">The relative script path from the base script folder.</param>
        public static String Script(this UrlHelper url, String relativePath)
        {
            return url.Content("~/" + (url.RequestContext.HttpContext.IsDebuggingEnabled ? DebugContent : ReleaseContent) + "/js/" + relativePath);
        }

        /// <summary>
        /// Get the stylesheet url based on the specified <paramref name="relativePath"/>.
        /// </summary>
        /// <param name="url">The <see cref="UrlHelper"/> instance to extend.</param>
        /// <param name="relativePath">The relative stylesheet path from the base stylesheet folder.</param>
        public static String StyleSheet(this UrlHelper url, String relativePath)
        {
            return url.Content("~/" + (url.RequestContext.HttpContext.IsDebuggingEnabled ? DebugContent : ReleaseContent) + "/css/" + relativePath);
        }

        /// <summary>
        /// Get the image url based on the specified <paramref name="relativePath"/>.
        /// </summary>
        /// <param name="url">The <see cref="UrlHelper"/> instance to extend.</param>
        /// <param name="relativePath">The relative image path from the base image folder.</param>
        public static String Image(this UrlHelper url, String relativePath)
        {
            return url.Content("~/" + (url.RequestContext.HttpContext.IsDebuggingEnabled ? DebugContent : ReleaseContent) + "/img/" + relativePath);
        }
    }
}