using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Results
{
    public class WebPartAnalysisResult
    {
        public string WebPartControlId { get; set; } = null!;

        public string WebPartType { get; set; } = null!;

        public int PageTemplateId { get; set; }

        [JsonIgnore]
        public IEnumerable<CmsTreeNode> TreeNodes { get; set; } = null!;

        public int TreeNodeCount => TreeNodes.Count();
    }
}