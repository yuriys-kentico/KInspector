namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public static class Scripts
    {
        public readonly static string BaseDirectory = $"{nameof(WebPartPerformanceAnalysis)}/Scripts/";
        public readonly static string GetCmsPageTemplatesWithWebPartsWithColumnsProperty = $"{BaseDirectory}{nameof(GetCmsPageTemplatesWithWebPartsWithColumnsProperty)}.sql";
        public readonly static string GetTreeNodesUsingPageTemplates = $"{BaseDirectory}{nameof(GetTreeNodesUsingPageTemplates)}.sql";
    }
}