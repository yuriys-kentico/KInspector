namespace KenticoInspector.Reports.UserPasswordAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(UserPasswordAnalysis)}/Scripts";

        public static string GetEnabledAndNotExternalUsers =
            $"{baseDirectory}/{nameof(GetEnabledAndNotExternalUsers)}.sql";
    }
}