using System.Collections.Generic;

using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Repositories
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<IReport> GetReports();
    }
}