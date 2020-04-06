namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsTreeNode
    {
        public int NodeID { get; set; }

        public string NodeName { get; set; } = null!;

        public int? NodeParentID { get; set; }

        public int NodeSiteID { get; set; }

        public string NodeAliasPath { get; set; } = null!;

        public int NodeLevel { get; set; }

        public int NodeClassID { get; set; }

        public string ClassDisplayName { get; set; } = null!;

        public string ClassName { get; set; } = null!;
    }
}