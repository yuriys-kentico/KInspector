using System;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Instances.Repositories
{
    public interface IVersionRepository : IRepository
    {
        Version GetKenticoAdministrationVersion(Instance instance);

        Version GetKenticoAdministrationVersion(string rootPath);

        Version GetKenticoDatabaseVersion(Instance instance);

        Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings);
    }
}