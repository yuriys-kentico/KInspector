using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
{
    public interface IModule
    {
        string CodeName { get; }

        string CompatibleVersions { get; }

        string IncompatibleVersions { get; }

        IList<string> Tags { get; }

        void SetModuleProperties(
            Func<Type, string> getCodeName,
            Func<Type, IModuleMetadata> getModuleMetadata,
            Func<Type, (string, string)> getSupportedVersions
            );
    }
}