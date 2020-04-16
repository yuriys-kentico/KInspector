namespace KenticoInspector.Reports.DatabaseConsistencyCheck
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(DatabaseConsistencyCheck)}/Scripts";

        public static string GetCheckDbResults = $"{baseDirectory}/{nameof(GetCheckDbResults)}.sql";
    }
}