using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using Dapper;

using KenticoInspector.Core;
using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Instances.Services
{
    public class DatabaseService : IDatabaseService
    {
        private const int CommandTimeout = 300;

        private IDbConnection? connection;

        private IDbConnection Connection => connection ?? throw new Exception($"You must run '{nameof(Configure)}' first.");

        public void Configure(DatabaseSettings databaseSettings)
        {
            var connectionString = GetConnectionString(databaseSettings);

            connection = new SqlConnection(connectionString);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, null, null);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, null, parameters);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFile<T>(relativeFilePath, literalReplacements, null);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = GetSqlQueryText(relativeFilePath, literalReplacements);

            return Connection.Query<T>(query, (object?)parameters, commandTimeout: CommandTimeout);
        }

        public DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath)
        {
            var query = GetSqlQueryText(relativeFilePath);
            var result = new DataTable();

            result.Load(Connection.ExecuteReader(query, commandTimeout: CommandTimeout));

            return result;
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, null);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic? parameters = null)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, null, parameters);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileGeneric(relativeFilePath, literalReplacements, null);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = GetSqlQueryText(relativeFilePath, literalReplacements);

            return Connection.Query(query, (object?)parameters, commandTimeout: CommandTimeout)
                .Select(x => (IDictionary<string, object>)x);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, null);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, null, parameters);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
        {
            return ExecuteSqlFromFileScalar<T>(relativeFilePath, literalReplacements, null);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, dynamic? parameters)
        {
            var query = GetSqlQueryText(relativeFilePath, literalReplacements);

            return Connection.QueryFirst<T>(query, (object?)parameters, commandTimeout: CommandTimeout);
        }

        private static string GetConnectionString(DatabaseSettings databaseSettings)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

            if (databaseSettings.IntegratedSecurity)
            {
                sb.IntegratedSecurity = true;
            }
            else
            {
                sb.UserID = databaseSettings.User;
                sb.Password = databaseSettings.Password;
            }

            sb["Server"] = databaseSettings.Server;
            sb["Database"] = databaseSettings.Database;

            return sb.ConnectionString;
        }

        /// <summary>
        /// Reads file located at <paramref name="relativeFilePath"/> using <see cref="CoreHelper.GetExecutingDirectory"/> as the root location.
        /// </summary>
        /// <param name="relativeFilePath">Relative file path not starting with a slash (/).</param>
        /// <param name="literalReplacements">Dictionary of string replacements.</param>
        /// <returns></returns>
        private static string GetSqlQueryText(string relativeFilePath, IDictionary<string, string>? literalReplacements = null)
        {
            var executingDirectory = CoreHelper.GetExecutingDirectory();

            var fullPathToScript = $"{executingDirectory}/{relativeFilePath}";

            var query = File.ReadAllText(fullPathToScript);

            if (literalReplacements != null)
            {
                foreach (var replacement in literalReplacements)
                {
                    query = query.Replace(replacement.Key, replacement.Value);
                }
            }

            return query;
        }
    }
}