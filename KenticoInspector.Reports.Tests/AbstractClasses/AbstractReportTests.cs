﻿using System;
using System.Reflection;

using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Core.Tokens;
using KenticoInspector.Reports.Tests.Helpers;

using Moq;

namespace KenticoInspector.Reports.Tests
{
    public abstract class AbstractReportTests<ReportType, TermsType>
        where ReportType : AbstractReport<TermsType>
        where TermsType : new()
    {
        protected Instance mockInstance;
        protected InstanceDetails mockInstanceDetails;
        protected Mock<IDatabaseService> mockDatabaseService;
        protected Mock<IInstanceService> mockInstanceService;
        protected Mock<ICmsFileService> mockCmsFileService;

        protected AbstractReportTests(int majorVersion)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(TokenExpressionResolver).Assembly);

            mockInstance = MockInstances.Get(majorVersion);
            mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);
            mockInstanceService = MockInstanceServiceHelper.SetupInstanceService(mockInstance, mockInstanceDetails);
            mockDatabaseService = MockDatabaseServiceHelper.SetupMockDatabaseService(mockInstance);
            mockCmsFileService = MockCmsFileServiceHelper.SetupMockCmsFileService();
        }

        protected T ArrangeProperties<T>(T module) where T : IModule
        {
            var moduleType = module.GetType();

            var fakeMetadata = new ModuleMetadata<TermsType>()
            {
                Terms = new TermsType()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            module.SetModuleProperties(GetCodeName, (moduleType, codeName) => fakeMetadata);

            return module;
        }

        private static string GetCodeName(Type moduleType)
        {
            var fullNameSpace = moduleType.Namespace
                ?? throw new InvalidOperationException($"Type '{moduleType}' does not have a namespace.");

            var indexAfterLastPeriod = fullNameSpace.LastIndexOf('.') + 1;

            return fullNameSpace[indexAfterLastPeriod..];
        }

        private static void UpdatePropertiesOfObject<T>(T objectToUpdate) where T : new()
        {
            var objectProperties = objectToUpdate?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (objectProperties != null)
            {
                foreach (var property in objectProperties)
                {
                    if (property.PropertyType == typeof(Term))
                    {
                        property.SetValue(objectToUpdate, (Term)property.Name);
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        var childObject = Activator.CreateInstance(property.PropertyType);
                        UpdatePropertiesOfObject(childObject);
                        property.SetValue(objectToUpdate, childObject);
                    }
                }
            }
        }
    }
}