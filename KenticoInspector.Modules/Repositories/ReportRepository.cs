using System.Collections.Generic;

using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Modules.Repositories;

namespace KenticoInspector.Modules.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IEnumerable<IReport> reports;

        public ReportRepository(IEnumerable<IReport> reports)
        {
            this.reports = reports;
        }

        public IEnumerable<IReport> GetReports() => reports;
    }
}