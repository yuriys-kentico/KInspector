using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationUsageResult : CmsPageTemplate
    {
        public string WebPartControlId { get; }

        public string WebPartPropertyName { get; }

        public string TransformationFullName { get; }

        public string TransformationType { get; }

        public TransformationUsageResult(CmsPageTemplate pageTemplate, WebPart webPart, WebPartProperty webPartProperty, CmsTransformation transformation)
        {
            PageTemplateID = pageTemplate.PageTemplateID;
            PageTemplateCodeName = pageTemplate.PageTemplateCodeName;
            PageTemplateDisplayName = pageTemplate.PageTemplateDisplayName;
            WebPartControlId = webPart.ControlId;
            WebPartPropertyName = webPartProperty.Name;
            TransformationFullName = transformation.FullName;
            TransformationType = transformation.TransformationType.ToString();
        }
    }
}