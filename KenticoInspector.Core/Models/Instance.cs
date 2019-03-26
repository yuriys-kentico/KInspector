﻿using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Models
{
    public class Instance
    {
        public DatabaseSettings DatabaseSettings { get; set; }
        public Guid Guid { get; set; }
        public Version KenticoAdministrationVersion { get; set; }
        public Version KenticoDatabaseVersion { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
