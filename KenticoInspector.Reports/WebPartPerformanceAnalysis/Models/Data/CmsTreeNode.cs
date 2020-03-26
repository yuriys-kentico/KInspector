using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data
{
    public class CmsTreeNode
    {
        public string DocumentName { get; set; }

        public int DocumentPageTemplateID { get; set; }

        [JsonIgnore]
        public string DocumentWebParts { get; set; }

        public string NodeAliasPath { get; set; }

        public int NodeSiteID { get; set; }
    }
}