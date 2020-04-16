using System;
using System.Collections.Generic;

using KenticoInspector.Core.Modules.Models;

namespace KenticoInspector.Core.Modules.Services
{
    public interface IModuleMetadataService : IService
    {
        string DefaultCultureName { get; }

        string CurrentCultureName { get; }

        IModuleMetadata GetModuleMetadata(
            string moduleCodename,
            Type metadataTermsType,
            IEnumerable<Tags> tags
            );
    }
}