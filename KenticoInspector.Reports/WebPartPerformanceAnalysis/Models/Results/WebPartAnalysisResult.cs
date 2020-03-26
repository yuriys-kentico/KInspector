using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class WebPartAnalysisResult
    {
        public string WebPartControlId { get; set; }

        public string WebPartName { get; set; }

        public string WebPartType { get; set; }

        public int PageTemplateId { get; set; }

        [JsonIgnore]
        public IEnumerable<CmsTreeNode> TreeNodes { get; set; }

        public int TreeNodeCount => TreeNodes.Count();
    }
}