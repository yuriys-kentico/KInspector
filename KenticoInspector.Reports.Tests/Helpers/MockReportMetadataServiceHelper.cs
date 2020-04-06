using System;
using System.Reflection;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Moq;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockModuleMetadataServiceHelper
    {
        public static Mock<IModuleMetadataService> GetModuleMetadataService()
        {
            return new Mock<IModuleMetadataService>(MockBehavior.Strict);
        }

        /// <summary>
        /// Sets up <see cref="IModuleMetadataService"/> to return a new <see cref="ModuleMetadata.Terms"/> instead of the real metadata.
        /// This is because the metadata does not influence the data retrieved by the report.
        /// </summary>
        /// <param name="mockModuleMetadataService">Mocked <see cref="IModuleMetadataService"/>.</param>
        /// <param name="report"><see cref="IReport"/> being tested.</param>
        /// <returns><see cref="IModuleMetadataService"/> configured for the <see cref="IReport"/>.</returns>
        public static void SetupModuleMetadataService<T>(Mock<IModuleMetadataService> mockModuleMetadataService, IReport report) where T : new()
        {
            SetupModuleMetadataServiceInternal<T>(report.Codename, mockModuleMetadataService);
        }

        public static Mock<IModuleMetadataService> GetBasicModuleMetadataService<T>(string reportCodename) where T : new()
        {
            var mockModuleMetadataService = GetModuleMetadataService();

            SetupModuleMetadataServiceInternal<T>(reportCodename, mockModuleMetadataService);

            return mockModuleMetadataService;
        }

        private static void SetupModuleMetadataServiceInternal<T>(string reportCodename, Mock<IModuleMetadataService> mockModuleMetadataService) where T : new()
        {
            var fakeMetadata = new ModuleMetadata<T>()
            {
                Terms = new T()
            };

            UpdatePropertiesOfObject(fakeMetadata.Terms);

            mockModuleMetadataService.Setup(p => p.GetModuleMetadata<T>(reportCodename)).Returns(fakeMetadata);
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