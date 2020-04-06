using System;

namespace KenticoInspector.Core.Models
{
    public class Instance
    {
        public DatabaseSettings DatabaseSettings { get; set; } = null!;

        public Guid Guid { get; set; }

        public string Name { get; set; } = null!;

        public string Path { get; set; } = null!;

        public string Url { get; set; } = null!;
    }
}