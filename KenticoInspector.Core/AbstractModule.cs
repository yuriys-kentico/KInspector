using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core
{
    public abstract class AbstractModule<T> : IModule where T : new()
    {
        public string CodeName { get; private set; } = null!;

        public string CompatibleVersions { get; private set; } = null!;

        public string IncompatibleVersions { get; private set; } = null!;

        public abstract IList<string> Tags { get; }

        public ModuleMetadata<T> Metadata { get; private set; } = null!;

        public void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, IModuleMetadata> getModuleMetadata,
            Func<Type, (string, string)> getSupportedVersions
            )
        {
            var moduleType = GetType();

            CodeName = getCodeName(moduleType);
            Metadata = (ModuleMetadata<T>)getModuleMetadata(moduleType);
            (CompatibleVersions, IncompatibleVersions) = getSupportedVersions(moduleType);
        }
    }
}