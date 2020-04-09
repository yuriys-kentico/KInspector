using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Repositories;
using KenticoInspector.Core.Services;

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
                action.SetModuleProperties(
                    GetModuleCodeName,
                    GetSupportedVersions,
                    GetModuleMetadata
                    );
            }

            return actions;
        }

        public ActionResults GetActionResults(string reportCodeName, Guid instanceGuid, string optionsJson)
        {
            var instance = instanceService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            var action = actionRepository.GetActions()
                .Where(action => GetModuleCodeName(action.GetType()) == reportCodeName)
                .First();

            action.SetModuleProperties(
                GetModuleCodeName,
                GetSupportedVersions,
                GetModuleMetadata
                );

            return action.GetResults(optionsJson);
        }

        public IEnumerable<IReport> GetReports(Guid instanceGuid)
        {
            instanceService.SetCurrentInstance(instanceGuid);

            var reports = reportRepository.GetReports();

            foreach (var report in reports)
            {
                report.SetModuleProperties(
                    GetModuleCodeName,
                    GetSupportedVersions,
                    GetModuleMetadata
                    );
            }

            return reports;
        }

        public ReportResults GetReportResults(string reportCodeName, Guid instanceGuid)
        {
            var instance = instanceService.SetCurrentInstance(instanceGuid);
            databaseService.Configure(instance.DatabaseSettings);

            var report = reportRepository.GetReports()
                .Where(report => GetModuleCodeName(report.GetType()) == reportCodeName)
                .First();

            report.SetModuleProperties(
                GetModuleCodeName,
                GetSupportedVersions,
                GetModuleMetadata
                );

            return report.GetResults();
        }

        public string GetModuleCodeName(Type moduleType)
        {
            var fullNameSpace = moduleType.Namespace
                ?? throw new InvalidOperationException($"Type '{moduleType}' does not have a namespace.");

            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;

            return fullNameSpace[indexAfterLastPeriod..];
        }

        public (string CompatibleSemver, string IncompatibleSemver) GetSupportedVersions(Type moduleType)
        {
            if (typeof(IReport).IsAssignableFrom(moduleType))
            {
                var method = moduleType.GetRuntimeMethod(nameof(IReport.GetResults), Array.Empty<Type>())
                    ?? throw new InvalidOperationException($"Report of type '{moduleType}' does not have '{nameof(IReport.GetResults)}'.");

                var attribute = method.GetCustomAttribute<SupportsVersionsAttribute>()
                    ?? throw new InvalidOperationException($"Report of type '{moduleType}' does not have a '{nameof(SupportsVersionsAttribute)}' on '{nameof(IReport.GetResults)}'.");

                return (attribute.CompatibleSemver, attribute.IncompatibleSemver);
            }

            if (typeof(IAction).IsAssignableFrom(moduleType))
            {
                var optionsType = moduleType.BaseType?.GetGenericArguments()[1]
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have an options type.");

                var method = moduleType.GetRuntimeMethod(nameof(IAction.GetResults), new[] { optionsType })
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have '{nameof(IAction.GetResults)}'.");

                var attribute = method.GetCustomAttribute<SupportsVersionsAttribute>()
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have a '{nameof(SupportsVersionsAttribute)}' on '{nameof(IAction.GetResults)}'.");

                return (attribute.CompatibleSemver, attribute.IncompatibleSemver);
            }

            throw new InvalidOperationException($"Module of type '{moduleType}' does not implement '{nameof(IReport)}' or '{nameof(IAction)}'.");
        }

        public IModuleMetadata GetModuleMetadata(Type moduleType)
        {
            var moduleBaseType = moduleType.BaseType
                ?? throw new InvalidOperationException($"Module of type '{moduleType}' does not have a base type.");

            IEnumerable<Tags> tags;

            if (typeof(IReport).IsAssignableFrom(moduleType))
            {
                var method = moduleType.GetRuntimeMethod(nameof(IReport.GetResults), Array.Empty<Type>())
                    ?? throw new InvalidOperationException($"Report of type '{moduleType}' does not have '{nameof(IReport.GetResults)}'.");

                var attribute = method.GetCustomAttribute<TagsAttribute>()
                    ?? throw new InvalidOperationException($"Report of type '{moduleType}' does not have a '{nameof(TagsAttribute)}' on '{nameof(IReport.GetResults)}'.");

                tags = attribute.Tags;
            }
            else if (typeof(IAction).IsAssignableFrom(moduleType))
            {
                var optionsType = moduleType.BaseType?.GetGenericArguments()[1]
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have an options type.");

                var method = moduleType.GetRuntimeMethod(nameof(IAction.GetResults), new[] { optionsType })
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have '{nameof(IAction.GetResults)}'.");

                var attribute = method.GetCustomAttribute<TagsAttribute>()
                    ?? throw new InvalidOperationException($"Action of type '{moduleType}' does not have a '{nameof(TagsAttribute)}' on '{nameof(IAction.GetResults)}'.");

                tags = attribute.Tags;
            }
            else
            {
                throw new InvalidOperationException($"Module of type '{moduleType}' does not implement '{nameof(IReport)}' or '{nameof(IAction)}'.");
            }

            return moduleMetadataService.GetModuleMetadata(
                GetModuleCodeName(moduleType),
                moduleBaseType.GetGenericArguments()[0],
                tags
            );
        }
    }
}