namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(PageTypeAssignmentAnalysis)}/Scripts";

        public static string GetPageTypesNotAssignedToSite =
            $"{baseDirectory}/{nameof(GetPageTypesNotAssignedToSite)}.sql";
    }
}