﻿using System;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IModuleMetadataService : IService
    {
        string DefaultCultureName { get; }

        string CurrentCultureName { get; }

        IModuleMetadata GetModuleMetadata(string moduleCodename, Type metadataTermsType);
    }
}