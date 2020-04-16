namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(SecuritySettingsAnalysis)}/Scripts";

        public static string GetSecurityCmsSettings = $"{baseDirectory}/{nameof(GetSecurityCmsSettings)}.sql";
        public static string GetCmsSettingsCategories = $"{baseDirectory}/{nameof(GetCmsSettingsCategories)}.sql";
    }
}