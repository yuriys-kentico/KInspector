using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Results
{
    public class TemplateAnalysisResult : CmsPageTemplate
    {
        [JsonIgnore]
        public IEnumerable<CmsTreeNode> TreeNodesWithIssues { get; set; } = null!;

        [JsonIgnore]
        public IEnumerable<WebPartAnalysisResult> WebPartsWithIssues { get; set; } = null!;

        public int TotalAffectedTreeNodes => TreeNodesWithIssues.Count();

        public int TotalAffectedWebParts => WebPartsWithIssues.Count();
    }
}