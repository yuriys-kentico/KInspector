using System;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Infrastructure.Models;

namespace KenticoInspector.Infrastructure
{
    public abstract class AbstractModule<T> : IModule where T : new()
    {
        public string CodeName { get; private set; } = null!;

        public string CompatibleVersions { get; private set; } = null!;

        public string IncompatibleVersions { get; private set; } = null!;

        public ModuleMetadata<T> Metadata { get; private set; } = null!;

        public void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, (string, string)> getSupportedVersions,
            Func<Type, IModuleMetadata> getModuleMetadata
            )
        {
            var moduleType = GetType();

            CodeName = getCodeName(moduleType);
            Metadata = (ModuleMetadata<T>)getModuleMetadata(moduleType);
            (CompatibleVersions, IncompatibleVersions) = getSupportedVersions(moduleType);
        }
    }
}