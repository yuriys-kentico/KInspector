namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(ApplicationRestartAnalysis)}/Scripts";

        public static string GetCmsEventLogsWithStartOrEndCode =
            $"{baseDirectory}/{nameof(GetCmsEventLogsWithStartOrEndCode)}.sql";
    }
}