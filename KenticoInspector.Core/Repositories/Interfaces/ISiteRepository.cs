using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        Site GetSite(Instance instance, int siteId);

        IList<Site> GetSites(Instance instance);
    }
}