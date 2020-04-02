using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IModuleService : IService
    {
        IReport GetReport(string codename);

        IEnumerable<IReport> GetReports(Guid instanceGuid);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IAction GetAction(string codename);

        IEnumerable<IAction> GetActions(Guid instanceGuid);

        ActionResults ExecuteAction(string actionCodename, Guid instanceGuid, string optionsJson);
    }
}