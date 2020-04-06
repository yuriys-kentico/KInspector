using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; } = null!;

        public Term GoodSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term DocumentsWithNoTreeNode { get; set; } = null!;

        public Term TreeNodesWithABadParentNode { get; set; } = null!;

        public Term TreeNodesWithABadParentSite { get; set; } = null!;

        public Term TreeNodesWithDuplicatedAliasPath { get; set; } = null!;

        public Term TreeNodesWithLevelInconsistencyAliasPath { get; set; } = null!;

        public Term TreeNodesWithLevelInconsistencyParent { get; set; } = null!;

        public Term TreeNodesWithNoDocumentNode { get; set; } = null!;

        public Term TreeNodesWithPageTypeNotAssignedToSite { get; set; } = null!;

        public Term WorkflowInconsistencies { get; set; } = null!;
    }
}