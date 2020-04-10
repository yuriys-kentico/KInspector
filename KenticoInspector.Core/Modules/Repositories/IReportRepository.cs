using System.Collections.Generic;

using KenticoInspector.Core.Modules.Models;

namespace KenticoInspector.Core.Modules.Repositories
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();
    }
}