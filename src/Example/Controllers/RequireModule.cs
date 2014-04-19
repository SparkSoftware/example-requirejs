using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spark.Example.MultiPageMvcWithRequireJs.Controllers
{
    /// <summary>
    /// RequireJS helper class.
    /// </summary>
    internal static class RequireModule
    {
        private static readonly IDictionary<String, IDictionary<String, String>> Controllers = new Dictionary<String, IDictionary<String, String>>();

        /// <summary>
        /// Get the case-sensitive RequireJS module name based on the specified base <paramref name="controller"/> and <paramref name="action"/>.
        /// </summary>
        /// <param name="path">The JavaScript content folder physical path.</param>
        /// <param name="controller">The controller name.</param>
        /// <param name="action">The action name.</param>
        public static String GetModuleName(String path, String controller, String action)
        {
            IDictionary<String, String> actions;
            String moduleName;

            lock (Controllers)
            {
                if (!Controllers.TryGetValue(controller, out actions))
                    Controllers.Add(controller, actions = new Dictionary<String, String>());
            }

            lock (actions)
            {
                if (!actions.TryGetValue(action, out moduleName))
                    actions[action] = moduleName = DiscoverModuleName(path, controller, action);
            }

            return moduleName;
        }

        /// <summary>
        /// Discover teh case-sensitive module name for the specified <paramref name="controller"/> and <paramref name="action"/>.
        /// </summary>
        /// <param name="path">The JavaScript content folder physical path.</param>
        /// <param name="controller">The controller name.</param>
        /// <param name="action">The action name.</param>
        private static String DiscoverModuleName(String path, String controller, String action)
        {
            var viewPath = Path.Combine(path, "view");
            var controllerPath = Path.Combine(viewPath, controller);
            var actionPath = Path.Combine(controllerPath, action);
            var moduleName = GetDirectoryName(viewPath) + '/' + GetDirectoryName(controllerPath) + '/' + GetDirectoryName(actionPath) + '/' + GetFileName("main.js");

            return moduleName;
        }

        /// <summary>
        /// Get the case-sensitive directory name for the <paramref name="path"/> specified.
        /// </summary>
        /// <param name="path">The directory path.</param>
        private static String GetDirectoryName(String path)
        {
            var info = new DirectoryInfo(path);

            return info.Exists && info.Parent != null ? info.Parent.GetDirectories(info.Name).Single().Name : info.Name;
        }

        /// <summary>
        /// Get the case-sensitive file name for the <paramref name="path"/> specified.
        /// </summary>
        /// <param name="path">The directory path.</param>
        private static String GetFileName(String path)
        {
            var info = new FileInfo(path);

            return Path.GetFileNameWithoutExtension(info.Exists && info.Directory != null ? info.Directory.GetFiles(info.Name).Single().Name : info.Name);
        }
    }
}