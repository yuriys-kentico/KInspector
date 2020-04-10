using System;

namespace KenticoInspector.Core.Modules.Models
{
    public interface IModule
    {
        string CodeName { get; }

        string CompatibleVersions { get; }

        string IncompatibleVersions { get; }

        void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, (string, string)> getSupportedVersions,
            Func<Type, IModuleMetadata> getModuleMetadata
            );
    }
}