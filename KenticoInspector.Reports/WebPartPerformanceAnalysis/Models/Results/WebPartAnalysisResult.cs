using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class WebPartAnalysisResult
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int TemplateId { get; set; }

        [JsonIgnore]
        public IEnumerable<Document> Documents { get; set; }

        public int DocumentCount => Documents.Count();
    }
}