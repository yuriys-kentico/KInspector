using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationUsageResult : CmsPageTemplate
    {
        [JsonProperty(Order = 1)]
        public string WebPartControlId { get; set; } = null!;

        [JsonProperty(Order = 2)]
        public string WebPartPropertyName { get; set; } = null!;

        [JsonProperty(Order = 3)]
        public int TransformationID { get; set; }

        [JsonProperty(Order = 4)]
        public string TransformationFullName { get; set; } = null!;
    }
}