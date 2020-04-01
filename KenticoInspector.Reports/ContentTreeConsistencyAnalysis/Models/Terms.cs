﻿using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; }

        public Term GoodSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }
    }

    public class TableNamesTerms
    {
        public Term DocumentsWithNoTreeNode { get; set; }

        public Term TreeNodesWithABadParentNode { get; set; }

        public Term TreeNodesWithABadParentSite { get; set; }

        public Term TreeNodesWithDuplicatedAliasPath { get; set; }

        public Term TreeNodesWithLevelInconsistencyAliasPath { get; set; }

        public Term TreeNodesWithLevelInconsistencyParent { get; set; }

        public Term TreeNodesWithNoDocumentNode { get; set; }

        public Term TreeNodesWithPageTypeNotAssignedToSite { get; set; }

        public Term WorkflowInconsistencies { get; set; }
    }
}