namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(TaskProcessingAnalysis)}/Scripts";

        public static string GetCmsScheduledTasksInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsScheduledTasksInPast24Hours)}.sql";
        public static string GetCmsSearchTasksInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsSearchTasksInPast24Hours)}.sql";
        public static string GetCmsIntegrationTasksInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsIntegrationTasksInPast24Hours)}.sql";
        public static string GetCmsStagingTasksInpast24Hours = $"{BaseDirectory}/{nameof(GetCmsStagingTasksInpast24Hours)}.sql";
        public static string GetCmsWebFarmTasksInPast24Hours = $"{BaseDirectory}/{nameof(GetCmsWebFarmTasksInPast24Hours)}.sql";
    }
}