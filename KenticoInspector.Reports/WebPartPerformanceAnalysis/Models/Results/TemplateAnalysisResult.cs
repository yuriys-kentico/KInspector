using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class TemplateAnalysisResult : CmsPageTemplate
    {
        [JsonIgnore]
        public IEnumerable<TreeNode> TreeNodesWithIssues { get; set; }

        [JsonIgnore]
        public IEnumerable<WebPartAnalysisResult> WebPartsWithIssues { get; set; }

        public int TotalAffectedTreeNodes => TreeNodesWithIssues.Count();

        public int TotalAffectedWebParts => WebPartsWithIssues.Count();
    }
}