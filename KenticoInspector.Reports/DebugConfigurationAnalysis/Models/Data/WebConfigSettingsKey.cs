namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data
{
    public class WebConfigSettingsKey
    {
        public string KeyName { get; set; } = null!;

        public string KeyDisplayName { get; set; } = null!;

        public bool KeyValue { get; set; }

        public bool KeyDefaultValue { get; set; }
    }
}