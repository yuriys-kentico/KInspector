namespace KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Data
{
    public class CmsPageTemplate
    {
        public int PageTemplateID { get; set; }

        public string PageTemplateCodeName { get; set; } = null!;

        public string? PageTemplateLayout { get; set; }
    }
}