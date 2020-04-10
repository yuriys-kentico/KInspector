using System.Collections.Generic;

namespace KenticoInspector.Core.Modules.Models
{
    public interface IModuleMetadata
    {
        ModuleDetails Details { get; set; }

        Dictionary<string, string> Tags { get; set; }
    }
}