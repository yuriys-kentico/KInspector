using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;

using Newtonsoft.Json;

namespace KenticoInspector.Core
{
    public abstract class AbstractModule<T> : IModule where T : new()
    {
        public string CodeName { get; private set; } = null!;

        public abstract IList<Version> CompatibleVersions { get; }

        public virtual IList<Version> IncompatibleVersions => new List<Version>();

        public abstract IList<string> Tags { get; }

        public ModuleMetadata<T> Metadata => (ModuleMetadata<T>)ModuleMetadata;

        [JsonIgnore]
        public IModuleMetadata ModuleMetadata { get; private set; } = null!;

        public void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, string, IModuleMetadata> getModuleMetadata
            )
        {
            var moduleType = GetType();

            CodeName = getCodeName(moduleType);
            ModuleMetadata = getModuleMetadata(moduleType, CodeName);
        }
    }
}