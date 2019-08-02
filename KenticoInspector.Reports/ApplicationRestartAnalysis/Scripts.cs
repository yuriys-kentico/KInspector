namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class Scripts
    {
        public static string BaseDirectory = $"{nameof(ApplicationRestartAnalysis)}/Scripts";

        public static string GetEventLog = $"{BaseDirectory}/{nameof(GetEventLog)}.sql";
    }
}