using System;
using System.Collections.Generic;
using System.Data;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Instances.Services
{
    public interface IDatabaseService : IService
    {
        void Configure(DatabaseSettings databaseSettings);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, object parameters);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, object parameters);

        [Obsolete("A last resort when it is impossible to create a data model or use one of the generic options.")]
        DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, object parameters);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, IDictionary<string, string> literalReplacements);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, IDictionary<string, string> literalReplacements, object parameters);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, object parameters);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, object parameters);
    }
}