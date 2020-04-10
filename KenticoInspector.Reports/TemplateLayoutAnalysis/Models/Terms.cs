﻿using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public Term InformationSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term IdenticalPageLayouts { get; set; } = null!;
    }
}