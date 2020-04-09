using System;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
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