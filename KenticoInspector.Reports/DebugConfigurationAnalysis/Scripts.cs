namespace KenticoInspector.Reports.DebugConfigurationAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(DebugConfigurationAnalysis)}/Scripts";

        public static string GetCMSSettingsKeysForDebug = $"{baseDirectory}/{nameof(GetCMSSettingsKeysForDebug)}.sql";
    }
}