﻿using System.Xml.Linq;

namespace KenticoInspector.Reports.ColumnFieldValidation.Models.Data
{
    public class CmsClass
    {
        public int ClassID { get; set; }

        public string ClassName { get; set; } = null!;

        public string ClassDisplayName { get; set; } = null!;

        public string? ClassTableName { get; set; }

        public XDocument ClassXmlSchema { get; set; } = null!;
    }
}