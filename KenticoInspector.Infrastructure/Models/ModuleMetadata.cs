using System.Collections.Generic;

using KenticoInspector.Core.Models;

using Newtonsoft.Json;

namespace KenticoInspector.Infrastructure.Models
{
    public class ModuleMetadata<T> : IModuleMetadata where T : new()
    {
        public ModuleDetails Details { get; set; } = new ModuleDetails();

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public T Terms { get; set; } = new T();
    }
}