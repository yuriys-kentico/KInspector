using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories
{
    public interface ISiteRepository : IRepository
    {
        CmsSite GetSite(Instance instance, int siteId);

        IEnumerable<CmsSite> GetSites(Instance instance);
    }
}