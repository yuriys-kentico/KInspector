using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Instances.Models
{
    public class InstanceDetails
    {
        public Guid Guid { get; set; }

        public Version AdministrationVersion { get; set; } = null!;

        public Version DatabaseVersion { get; set; } = null!;

        public IEnumerable<CmsSite> Sites { get; set; } = null!;
    }
}