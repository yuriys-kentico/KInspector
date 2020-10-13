using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Core
{
    public static class CoreHelper
    {
        private const string filePrefix = "file:\\";

        /// <summary>
        ///     Gets the executing directory of the application.
        /// </summary>
        /// <returns>A string that contains the path of the executing directory, and does not end with a backslash (\).</returns>
        public static string GetExecutingDirectory()
        {
            var assemblyPath = Assembly.GetExecutingAssembly()
                .CodeBase;

            var assemblyDirectory = Path.GetDirectoryName(assemblyPath) ?? throw new DirectoryNotFoundException();

            return assemblyDirectory.Substring(filePrefix.Length);
        }

        public static IEnumerable<T> GetItemsWhereInManyIds<T>(this IDatabaseService databaseService,
            IEnumerable<int> manyIds,
            string sqlScriptRelativeFilePath
            )
        {
            const int maximumCountInParameters = 500;

            var idsBatches = manyIds
                .Select((id, index) => (id, index))
                .GroupBy(group => group.index / maximumCountInParameters, group => group.id);

            var items = new List<T>();

            foreach (var idsBatch in idsBatches)
                items.AddRange(databaseService.ExecuteSqlFromFile<T>(
                    sqlScriptRelativeFilePath,
                    new
                    {
                        idsBatch
                    }
                    ));

            return items;
        }
    }
}