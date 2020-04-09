using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Services
{
    public interface IModuleService : IService
    {
        IEnumerable<IReport> GetReports(Guid instanceGuid);

        ReportResults GetReportResults(string reportCodename, Guid instanceGuid);

        IEnumerable<IAction> GetActions(Guid instanceGuid);

        ActionResults GetActionResults(string actionCodename, Guid instanceGuid, string optionsJson);

        string GetModuleCodeName(Type moduleType);

        (string CompatibleSemver, string IncompatibleSemver) GetSupportedVersions(Type moduleType);

        IModuleMetadata GetModuleMetadata(Type moduleType);
    }
}