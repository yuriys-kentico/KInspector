using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class CmsTreeNode
    {
        public int NodeID { get; set; }

        public string DocumentName { get; set; } = null!;

        public string DocumentCulture { get; set; } = null!;

        public string NodeAliasPath { get; set; } = null!;

        [JsonIgnore]
        public int NodeSiteID { get; set; }

        public int DocumentPageTemplateID { get; set; }
    }
}