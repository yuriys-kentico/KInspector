namespace KenticoInspector.Reports.TemplateLayoutAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(TemplateLayoutAnalysis)}/Scripts";

        public static string GetCmsPageTemplates = $"{baseDirectory}/{nameof(GetCmsPageTemplates)}.sql";
    }
}