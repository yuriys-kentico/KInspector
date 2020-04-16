using System.Collections.Generic;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Instances.Repositories
{
    public interface ISiteRepository : IRepository
    {
        CmsSite GetSite(
            Instance instance,
            int siteId
            );

        IEnumerable<CmsSite> GetSites(Instance instance);
    }
}