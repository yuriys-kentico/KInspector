namespace KenticoInspector.Reports.PageTypeFieldAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(PageTypeFieldAnalysis)}/Scripts";

        public static string GetCmsPageTypeFields = $"{baseDirectory}/{nameof(GetCmsPageTypeFields)}.sql";
    }
}