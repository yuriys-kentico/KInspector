using System.IO;
using System.Reflection;

namespace KenticoInspector.Core
{
    public static class CoreHelper
    {
        private const string filePrefix = "file:\\";

        /// <summary>
        /// Gets the executing directory of the application.
        /// </summary>
        /// <returns>A string that contains the path of the executing directory, and does not end with a backslash (\).</returns>
        public static string GetExecutingDirectory()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().CodeBase;

            var assemblyDirectory = Path.GetDirectoryName(assemblyPath) ?? throw new DirectoryNotFoundException();

            return assemblyDirectory.Substring(filePrefix.Length);
        }
    }
}