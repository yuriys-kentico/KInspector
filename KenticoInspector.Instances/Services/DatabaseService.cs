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
        private const int commandTimeout = 300;
        private IDbConnection? connection;

        private IDbConnection Connection => connection ?? throw new Exception($"You must run '{nameof(Configure)}' first.");

        public void Configure(DatabaseSettings databaseSettings)
        {
            var connectionString = GetConnectionString(databaseSettings);
            connection = new SqlConnection(connectionString);
        }

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath)
            => ExecuteSqlFromFile<T>(
                relativeFilePath,
                null,
                null
                );

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, object parameters)
            => ExecuteSqlFromFile<T>(
                relativeFilePath,
                null,
                parameters
                );

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
            => ExecuteSqlFromFile<T>(
                relativeFilePath,
                literalReplacements,
                null
                );

        public IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, object? parameters)
        {
            var query = GetSqlQueryText(
                relativeFilePath,
                literalReplacements
                );

            return Connection.Query<T>(
                query,
                parameters,
                commandTimeout: commandTimeout
                );
        }

        public DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath)
        {
            var query = GetSqlQueryText(relativeFilePath);
            var result = new DataTable();

            result.Load(
                Connection.ExecuteReader(
                    query,
                    commandTimeout: commandTimeout
                    )
                );

            return result;
        }

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath)
            => ExecuteSqlFromFile(
                relativeFilePath,
                null,
                null
                );

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, object parameters)
            => ExecuteSqlFromFile(
                relativeFilePath,
                null,
                parameters
                );

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, IDictionary<string, string> literalReplacements)
            => ExecuteSqlFromFile(
                relativeFilePath,
                literalReplacements,
                null
                );

        public IEnumerable<IDictionary<string, object>> ExecuteSqlFromFile(string relativeFilePath, IDictionary<string, string>? literalReplacements, object? parameters)
        {
            var query = GetSqlQueryText(
                relativeFilePath,
                literalReplacements
                );

            return Connection.Query(
                    query,
                    parameters,
                    commandTimeout: commandTimeout
                    )
                .Select(x => (IDictionary<string, object>)x);
        }

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath) => ExecuteSqlFromFileScalar<T>(
            relativeFilePath,
            null,
            null
            );

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, object parameters) => ExecuteSqlFromFileScalar<T>(
            relativeFilePath,
            null,
            parameters
            );

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements)
            => ExecuteSqlFromFileScalar<T>(
                relativeFilePath,
                literalReplacements,
                null
                );

        public T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string>? literalReplacements, object? parameters)
        {
            var query = GetSqlQueryText(
                relativeFilePath,
                literalReplacements
                );

            return Connection.QueryFirst<T>(
                query,
                parameters,
                commandTimeout: commandTimeout
                );
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

        private static string GetSqlQueryText(string relativeFilePath, IDictionary<string, string>? literalReplacements = null)
        {
            var executingDirectory = CoreHelper.GetExecutingDirectory();
            var fullPathToScript = $"{executingDirectory}/{relativeFilePath}";
            var query = File.ReadAllText(fullPathToScript);

            if (literalReplacements == null) return query;

            return literalReplacements.Aggregate(
                query,
                (current, replacement) => current.Replace(
                    replacement.Key,
                    replacement.Value
                    )
                );
        }
    }
}