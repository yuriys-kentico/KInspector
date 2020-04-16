namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(WebPartPerformanceAnalysis)}/Scripts/";

        public static readonly string GetCmsPageTemplatesWithWebPartsWithColumnsProperty =
            $"{baseDirectory}{nameof(GetCmsPageTemplatesWithWebPartsWithColumnsProperty)}.sql";

        public static readonly string GetTreeNodesUsingPageTemplates =
            $"{baseDirectory}{nameof(GetTreeNodesUsingPageTemplates)}.sql";
    }
}