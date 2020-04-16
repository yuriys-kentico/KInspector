using System;

using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Modules.Models;

namespace KenticoInspector.Modules
{
    public abstract class AbstractModule<T> : IModule
        where T : new()
    {
        public ModuleMetadata<T> Metadata { get; private set; } = null!;

        public string CodeName { get; internal set; } = null!;

        public string CompatibleVersions { get; private set; } = null!;

        public string IncompatibleVersions { get; private set; } = null!;

        public void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, (string, string)> getSupportedVersions,
            Func<Type, IModuleMetadata> getModuleMetadata
            )
        {
            var moduleType = GetType();
            CodeName = getCodeName(moduleType);
            Metadata = (ModuleMetadata<T>) getModuleMetadata(moduleType);
            (CompatibleVersions, IncompatibleVersions) = getSupportedVersions(moduleType);
        }
    }
}