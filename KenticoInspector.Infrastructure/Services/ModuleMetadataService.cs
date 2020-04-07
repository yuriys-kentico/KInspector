using System;
using System.IO;
using System.Threading;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class ModuleMetadataService : IModuleMetadataService
    {
        private readonly IInstanceService instanceService;

        public string DefaultCultureName => "en-US";

        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public ModuleMetadataService(IInstanceService instanceService)
        {
            this.instanceService = instanceService;
        }

        public IModuleMetadata GetModuleMetadata(string moduleCodename, Type metadataTermsType)
        {
            var metadataType = typeof(ModuleMetadata<>).MakeGenericType(metadataTermsType);
            var metadataDirectory = $"{DirectoryHelper.GetExecutingDirectory()}\\{moduleCodename}\\Metadata\\";

            var mergedMetadata = (IModuleMetadata?)Activator.CreateInstance(metadataType)
                ?? throw new InvalidOperationException($"Type '{metadataType}' could not be created.");

            var currentCultureIsDefaultCulture = CurrentCultureName == DefaultCultureName;

            var currentMetadata = DeserializeMetadataFromYamlFile(
                metadataType,
                metadataDirectory,
                CurrentCultureName,
                false
            ) ?? throw new InvalidOperationException($"Metadata of type '{metadataType}' in culture '{CurrentCultureName}' could not be deserialized.");

            if (!currentCultureIsDefaultCulture)
            {
                var defaultMetadata = DeserializeMetadataFromYamlFile(
                    metadataType,
                    metadataDirectory,
                    DefaultCultureName,
                    true
                ) ?? throw new InvalidOperationException($"Metadata of type '{metadataType}' in culture '{DefaultCultureName}' could not be deserialized.");

                mergedMetadata = GetMergedMetadata(mergedMetadata, defaultMetadata, currentMetadata);
            }

            var instanceDetails = instanceService.GetInstanceDetails(instanceService.CurrentInstance);

            var commonData = new
            {
                instanceUrl = instanceService.CurrentInstance.Url,
                administrationVersion = instanceDetails.AdministrationVersion,
                databaseVersion = instanceDetails.DatabaseVersion
            };

            var moduleMetadata = currentCultureIsDefaultCulture ? currentMetadata : mergedMetadata;

            Term name = moduleMetadata.Details.Name;
            moduleMetadata.Details.Name = name.With(commonData);

            Term shortDescription = moduleMetadata.Details.ShortDescription;
            moduleMetadata.Details.ShortDescription = shortDescription.With(commonData);

            Term longDescription = moduleMetadata.Details.LongDescription;
            moduleMetadata.Details.LongDescription = longDescription.With(commonData);

            return moduleMetadata;
        }

        private static IModuleMetadata? DeserializeMetadataFromYamlFile(
            Type metadataType,
            string metadataDirectory,
            string cultureName,
            bool ignoreUnmatchedProperties)
        {
            var moduleMetadataPath = $"{metadataDirectory}{cultureName}.yaml";

            var moduleMetadataPathExists = File.Exists(moduleMetadataPath);

            if (moduleMetadataPathExists)
            {
                var fileText = File.ReadAllText(moduleMetadataPath);

                return DeserializeYaml(metadataType, fileText, ignoreUnmatchedProperties);
            }

            return null;
        }

        private static IModuleMetadata? DeserializeYaml(
            Type metadataType,
            string yaml,
            bool ignoreUnmatchedProperties)
        {
            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);

            if (ignoreUnmatchedProperties)
            {
                deserializerBuilder.IgnoreUnmatchedProperties();
            }

            var deserializer = deserializerBuilder.Build();

            return deserializer.Deserialize(yaml, metadataType) as IModuleMetadata;
        }

        private static IModuleMetadata GetMergedMetadata(
            IModuleMetadata mergedMetadata,
            IModuleMetadata defaultMetadata,
            IModuleMetadata overrideMetadata)
        {
            mergedMetadata.Details.Name = overrideMetadata.Details.Name ?? defaultMetadata.Details.Name;
            mergedMetadata.Details.ShortDescription =
                overrideMetadata.Details.ShortDescription ?? defaultMetadata.Details.ShortDescription;
            mergedMetadata.Details.LongDescription =
                overrideMetadata.Details.LongDescription ?? defaultMetadata.Details.LongDescription;

            RecursivelySetPropertyValues(
                (mergedMetadata as dynamic).Terms.GetType(),
                (defaultMetadata as dynamic).Terms,
                (overrideMetadata as dynamic).Terms,
                (mergedMetadata as dynamic).Terms);

            return mergedMetadata;
        }

        private static void RecursivelySetPropertyValues(
            Type objectType,
            object? defaultObject,
            object? overrideObject,
            object? targetObject)
        {
            var objectTypeProperties = objectType.GetProperties();

            foreach (var objectTypeProperty in objectTypeProperties)
            {
                var objectTypePropertyType = objectTypeProperty.PropertyType;

                var defaultObjectPropertyValue = objectTypeProperty.GetValue(defaultObject);

                var overrideObjectPropertyValue = overrideObject != null
                    ? objectTypeProperty.GetValue(overrideObject)
                    : defaultObjectPropertyValue;

                if (objectTypePropertyType.Namespace == objectType.Namespace)
                {
                    var targetObjectPropertyValue = Activator.CreateInstance(objectTypePropertyType);

                    objectTypeProperty.SetValue(targetObject, targetObjectPropertyValue);

                    RecursivelySetPropertyValues(
                        objectTypePropertyType,
                        defaultObjectPropertyValue,
                        overrideObjectPropertyValue,
                        targetObjectPropertyValue);
                }
                else
                {
                    objectTypeProperty.SetValue(targetObject, overrideObjectPropertyValue);
                }
            }
        }
    }
}