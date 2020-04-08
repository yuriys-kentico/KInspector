using System;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories
{
    public interface IVersionRepository : IRepository
    {
        Version GetKenticoAdministrationVersion(Instance instance);

        Version GetKenticoAdministrationVersion(string rootPath);

        Version GetKenticoDatabaseVersion(Instance instance);

        Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings);
    }
}