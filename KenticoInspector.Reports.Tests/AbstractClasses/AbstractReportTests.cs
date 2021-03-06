﻿using System;
using System.Reflection;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Core.TokenExpressions;
using KenticoInspector.Core.TokenExpressions.Models;
using KenticoInspector.Modules;
using KenticoInspector.Modules.Models;

using Moq;

namespace KenticoInspector.Reports.Tests.AbstractClasses
{
    public abstract class AbstractReportTests<ReportType, TermsType>
        where ReportType : AbstractReport<TermsType>
        where TermsType : new()
    {
        protected Mock<ICmsFileService> mockCmsFileService;
        protected Mock<IDatabaseService> mockDatabaseService;
        protected Instance mockInstance;
        protected InstanceDetails mockInstanceDetails;
        protected Mock<IInstanceService> mockInstanceService;

        protected AbstractReportTests(int majorVersion)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(TokenExpressionResolver).Assembly);
            mockInstance = MockInstances.Get(majorVersion);

            mockInstanceDetails = MockInstanceDetails.Get(
                majorVersion,
                mockInstance
                );

            mockInstanceService = MockIInstanceService.Get();

            mockInstanceService.SetupCurrentInstance(
                mockInstance,
                mockInstanceDetails
                );

            mockDatabaseService = MockIDatabaseService.Get();
            mockDatabaseService.SetupConfigure(mockInstance);
            mockCmsFileService = MockICmsFileService.Get();
        }

        protected T ArrangeProperties<T>(T module)
            where T : IModule
        {
            var moduleType = module.GetType();

            var fakeMetadata = new ModuleMetadata<TermsType>
            {
                Terms = new TermsType()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            module.SetModuleProperties(
                GetModuleCodeName,
                moduleType => ("*", ""),
                moduleType => fakeMetadata
                );

            return module;
        }

        private static string GetModuleCodeName(Type moduleType)
        {
            var fullNameSpace = moduleType.Namespace
                ?? throw new InvalidOperationException($"Type '{moduleType}' does not have a namespace.");

            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;

            return fullNameSpace[indexAfterLastPeriod..];
        }

        private static void UpdatePropertiesOfObject<T>(T objectToUpdate)
            where T : new()
        {
            var objectProperties = objectToUpdate?.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (objectProperties != null)
                foreach (var property in objectProperties)
                    if (property.PropertyType == typeof(Term))
                    {
                        property.SetValue(
                            objectToUpdate,
                            (Term)property.Name
                            );
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        var childObject = Activator.CreateInstance(property.PropertyType);
                        UpdatePropertiesOfObject(childObject);

                        property.SetValue(
                            objectToUpdate,
                            childObject
                            );
                    }
        }
    }
}