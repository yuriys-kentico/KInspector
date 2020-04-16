namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(DatabaseTableSizeAnalysis)}/Scripts";

        public static string GetTop25LargestTables = $"{baseDirectory}/{nameof(GetTop25LargestTables)}.sql";
    }
}