using System.Collections.Generic;

using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Repositories;

namespace KenticoInspector.Infrastructure.Repositories
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