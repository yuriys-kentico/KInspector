using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// Represents a culture version of a node. References a <see cref="CmsPageTemplate"/> in <see cref="DocumentPageTemplateID"/>.
    /// </summary>
    public class CmsTreeNode
    {
        public int NodeID { get; set; }

        public string DocumentName { get; set; }

        public string DocumentCulture { get; set; }

        public string NodeAliasPath { get; set; }

        [JsonIgnore]
        public int NodeSiteID { get; set; }

        public int DocumentPageTemplateID { get; set; }
    }
}