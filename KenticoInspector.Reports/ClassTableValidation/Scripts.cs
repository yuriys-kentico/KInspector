namespace KenticoInspector.Reports.ClassTableValidation
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(ClassTableValidation)}/Scripts";

        public static string GetCmsClassesWithMissingTable =
            $"{baseDirectory}/{nameof(GetCmsClassesWithMissingTable)}.sql";

        public static string GetTablesWithMissingClass = $"{baseDirectory}/{nameof(GetTablesWithMissingClass)}.sql";
    }
}