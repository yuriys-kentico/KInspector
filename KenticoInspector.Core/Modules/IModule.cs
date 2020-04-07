using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
{
    public interface IModule
    {
        string CodeName { get; }

        IList<Version> CompatibleVersions { get; }

        IList<Version> IncompatibleVersions { get; }

        IList<string> Tags { get; }

        IModuleMetadata ModuleMetadata { get; }

        void SetModuleProperties(Func<Type, string> getCodeName, Func<Type, string, IModuleMetadata> getModuleMetadata);
    }
}