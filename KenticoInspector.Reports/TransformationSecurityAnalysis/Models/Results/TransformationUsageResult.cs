using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationUsageResult : CmsPageTemplate
    {
        [JsonProperty(Order = 1)]
        public string WebPartControlId { get; set; }

        [JsonProperty(Order = 2)]
        public string WebPartPropertyName { get; set; }

        [JsonProperty(Order = 3)]
        public int TransformationID { get; set; }

        [JsonProperty(Order = 4)]
        public string TransformationFullName { get; set; }

        public TransformationUsageResult(CmsPageTemplate pageTemplate, WebPart webPart, WebPartProperty webPartProperty, CmsTransformation transformation)
        {
            PageTemplateID = pageTemplate.PageTemplateID;
            PageTemplateCodeName = pageTemplate.PageTemplateCodeName;
            PageTemplateDisplayName = pageTemplate.PageTemplateDisplayName;
            PageTemplateWebParts = pageTemplate.PageTemplateWebParts;

            WebPartControlId = webPart.ControlId;
            WebPartPropertyName = webPartProperty.Name;

            TransformationID = transformation.TransformationID;
            TransformationFullName = transformation.FullName;
        }
    }
}