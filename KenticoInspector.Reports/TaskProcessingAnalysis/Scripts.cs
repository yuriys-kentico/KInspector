namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(TaskProcessingAnalysis)}/Scripts";

        public static string GetCmsScheduledTasksInPast24Hours =
            $"{baseDirectory}/{nameof(GetCmsScheduledTasksInPast24Hours)}.sql";

        public static string GetCmsSearchTasksInPast24Hours =
            $"{baseDirectory}/{nameof(GetCmsSearchTasksInPast24Hours)}.sql";

        public static string GetCmsIntegrationTasksInPast24Hours =
            $"{baseDirectory}/{nameof(GetCmsIntegrationTasksInPast24Hours)}.sql";

        public static string GetCmsStagingTasksInpast24Hours =
            $"{baseDirectory}/{nameof(GetCmsStagingTasksInpast24Hours)}.sql";

        public static string GetCmsWebFarmTasksInPast24Hours =
            $"{baseDirectory}/{nameof(GetCmsWebFarmTasksInPast24Hours)}.sql";
    }
}