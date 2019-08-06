namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(TaskProcessingAnalysis)}/Scripts";

        public static string GetIntegrationTasksCountInPast24Hours = $"{BaseDirectory}/{nameof(GetIntegrationTasksCountInPast24Hours)}.sql";
        public static string GetCmsScheduledTasksCountInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsScheduledTasksCountInPast24Hours)}.sql";
        public static string GetCmsSearchTasksCountInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsSearchTasksCountInPast24Hours)}.sql";
        public static string GetStagingTasksCountInpast24Hours = $"{BaseDirectory}/{nameof(GetStagingTasksCountInpast24Hours)}.sql";
        public static string GetCmsWebFarmTaskCountInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsWebFarmTaskCountInPast24Hours)}.sql";
    }
}