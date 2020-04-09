using System.Collections.Generic;

namespace KenticoInspector.Core.Models
{
    public interface IModuleMetadata
    {
        ModuleDetails Details { get; set; }

        Dictionary<string, string> Tags { get; set; }
    }
}