using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class TemplateAnalysisResult
    {
        [JsonIgnore]
        public IEnumerable<Document> AffectedDocuments { get; set; }

        [JsonIgnore]
        public IEnumerable<WebPartAnalysisResult> AffectedWebParts { get; set; }

        public string TemplateCodename { get; set; }

        public string TemplateName { get; set; }

        public int TemplateID { get; set; }

        public int TotalAffectedDocuments => AffectedDocuments.Count();

        public int TotalAffectedWebParts => AffectedWebParts.Count();
    }
}