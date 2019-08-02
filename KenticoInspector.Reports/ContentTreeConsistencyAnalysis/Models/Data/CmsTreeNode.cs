namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsTreeNode
    {
        public int NodeID { get; set; }

        public string NodeName { get; set; }

        public int? NodeParentID { get; set; }

        public int NodeSiteID { get; set; }

        public string NodeAliasPath { get; set; }

        public int NodeLevel { get; set; }

        public int NodeClassID { get; set; }

        public string ClassDisplayName { get; set; }

        public string ClassName { get; set; }
    }
}