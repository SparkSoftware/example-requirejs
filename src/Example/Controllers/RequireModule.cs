using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Spark.Example.MultiPageMvcWithRequireJs.Controllers
{
    /// <summary>
    /// RequireJS helper class.
    /// </summary>
    internal static class RequireModule
    {
        private static readonly ConcurrentDictionary<View, String> Views = new ConcurrentDictionary<View, String>();

        /// <summary>
        /// Get the case-sensitive RequireJS module name based on the specified base <paramref name="controller"/> and <paramref name="action"/>.
        /// </summary>
        /// <param name="basePath">The base content folder physical path.</param>
        /// <param name="area">The optional area name.</param>
        /// <param name="controller">The controller name.</param>
        /// <param name="action">The action name.</param>
        public static String GetModuleName(String basePath, String area, String controller, String action)
        {
            View view = new View(controller, action, area);
            String moduleName;

            if (!Views.TryGetValue(view, out moduleName))
                Views.TryAdd(view, moduleName = DiscoverModuleName(basePath, area, controller, action));

            return moduleName;
        }

        /// <summary>
        /// Discover teh case-sensitive module name for the specified <paramref name="controller"/> and <paramref name="action"/>.
        /// </summary>
        /// <param name="path">The JavaScript content folder physical path.</param>
        /// <param name="controller">The controller name.</param>
        /// <param name="action">The controller action name.</param>
        /// <param name="area">The optional area name.</param>
        private static String DiscoverModuleName(String path, String controller, String action, String area)
        {
            String viewPath = Path.Combine(path, "view");
            String moduleName;

            if (String.IsNullOrWhiteSpace(area))
            {
                var controllerPath = Path.Combine(viewPath, controller);
                var actionFile = Path.Combine(controllerPath, Path.ChangeExtension(action, "js"));

                moduleName = GetDirectoryName(viewPath) + '/' + GetDirectoryName(controllerPath) + '/' + GetFileName(actionFile);
            }
            else
            {
                var areaPath = Path.Combine(viewPath, area);
                var controllerPath = Path.Combine(areaPath, controller);
                var actionFile = Path.Combine(controllerPath, Path.ChangeExtension(action, "js"));

                moduleName = GetDirectoryName(viewPath) + '/' + GetDirectoryName(areaPath) + '/' + GetDirectoryName(controllerPath) + '/' + GetFileName(actionFile);
            }

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

        /// <summary>
        /// Represents a specific action view within the application.
        /// </summary>
        private sealed class View : IEquatable<View>
        {
            private readonly String controller;
            private readonly String action;
            private readonly String area;

            /// <summary>
            /// Initializes a new instance of <see cref="View"/>.
            /// </summary>
            /// <param name="controller">The controller name.</param>
            /// <param name="action">The controller action name.</param>
            /// <param name="area">The optional area name.</param>
            public View(String controller, String action, String area)
            {
                this.controller = (controller ?? String.Empty).ToLowerInvariant();
                this.action = (action ?? String.Empty).ToLowerInvariant();
                this.area = (area ?? String.Empty).ToLowerInvariant();
            }

            /// <summary>
            /// Indicates whether this instance and a specified <see cref="Object"/> are equal.
            /// </summary>
            /// <param name="other">Another <see cref="Object"/> to compare.</param>
            public override Boolean Equals(Object other)
            {
                return Equals(other as View);
            }

            /// <summary>
            /// Indicates whether this instance and a specified <see cref="View"/> are equal.
            /// </summary>
            /// <param name="other">Another <see cref="View"/> to compare.</param>
            public Boolean Equals(View other)
            {
                return other != null && other.controller == controller && other.action == action && other.area == area;
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            public override Int32 GetHashCode()
            {
                unchecked
                {
                    var result = GetType().GetHashCode();

                    result = (result * 397) ^ controller.GetHashCode();
                    result = (result * 397) ^ action.GetHashCode();
                    result = (result * 397) ^ area.GetHashCode();

                    return result;
                }
            }

            /// <summary>
            /// Returns the <see cref="View"/> description for this instance.
            /// </summary>
            public override String ToString()
            {
                return String.Format("[{0},{1},{2}]", String.IsNullOrWhiteSpace(area) ? "n/a" : area, controller, action);
            }
        }
    }
}
