using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IReportRepository reportRepository;
        private readonly IActionRepository actionRepository;
        private readonly IInstanceService instanceService;
        private readonly IDatabaseService databaseService;
        private readonly IModuleMetadataService moduleMetadataService;

        public ModuleService(
            IReportRepository reportRepository,
            IActionRepository actionRepository,
            IInstanceService instanceService,
            IDatabaseService databaseService,
            IModuleMetadataService moduleMetadataService
            )
        {
            this.reportRepository = reportRepository;
            this.actionRepository = actionRepository;
            this.instanceService = instanceService;
            this.databaseService = databaseService;
            this.moduleMetadataService = moduleMetadataService;
        }

        public IEnumerable<IAction> GetActions(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);

            var actions = actionRepository.GetActions();

            foreach (var action in actions)
            {
                action.SetModuleProperties(GetCodeName, GetModuleMetadata);
            }

            return actions;
        }

        public ActionResults GetActionResults(string reportCodeName, Guid instanceGuid, string optionsJson)
        {
            var instance = instanceService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            var action = actionRepository.GetActions()
                .Where(action => GetCodeName(action.GetType()) == reportCodeName)
                .First();

            action.SetModuleProperties(GetCodeName, GetModuleMetadata);

            return action.GetResults(optionsJson);
        }

        public IEnumerable<IReport> GetReports(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);

            var reports = reportRepository.GetReports();

            foreach (var report in reports)
            {
                report.SetModuleProperties(GetCodeName, GetModuleMetadata);
            }

            return reports;
        }

        public ReportResults GetReportResults(string reportCodeName, Guid instanceGuid)
        {
            var instance = instanceService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            var report = reportRepository.GetReports()
                .Where(report => GetCodeName(report.GetType()) == reportCodeName)
                .First();

            report.SetModuleProperties(GetCodeName, GetModuleMetadata);

            return report.GetResults();
        }

        private static string GetCodeName(Type moduleType)
        {
            var fullNameSpace = moduleType.Namespace
                ?? throw new InvalidOperationException($"Type '{moduleType}' does not have a namespace.");

            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;

            return fullNameSpace[indexAfterLastPeriod..];
        }

        private IModuleMetadata GetModuleMetadata(Type moduleType, string moduleCodeName)
        {
            var moduleBaseType = moduleType.BaseType
                ?? throw new InvalidOperationException($"Module of type '{moduleType}' does not have a base type.");

            return moduleMetadataService.GetModuleMetadata(
                moduleCodeName,
                moduleBaseType.GetGenericArguments()[0]
            );
        }
    }
}