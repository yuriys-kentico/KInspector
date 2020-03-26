using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        CmsSite GetSite(Instance instance, int siteId);

        IEnumerable<CmsSite> GetSites(Instance instance);
    }
}