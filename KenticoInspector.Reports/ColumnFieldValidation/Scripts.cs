namespace KenticoInspector.Reports.ColumnFieldValidation
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(ColumnFieldValidation)}/Scripts";
        public static string GetCmsClasses = $"{baseDirectory}/{nameof(GetCmsClasses)}.sql";
        public static string GetTableColumns = $"{baseDirectory}/{nameof(GetTableColumns)}.sql";
    }
}