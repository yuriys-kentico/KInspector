namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data
{
    public class CmsSettingsKey
    {
        public int KeyID { get; set; }

        public string KeyName { get; set; }

        public string KeyDisplayName { get; set; }

        public bool KeyValue { get; set; }

        public bool KeyDefaultValue { get; set; }

        /// <summary>
        /// This constructor is required for deserialization.
        /// </summary>
        public CmsSettingsKey()
        {
        }

        public CmsSettingsKey(string keyName, string keyDisplayName, bool keyValue, bool keyDefaultValue)
        {
            KeyName = keyName;
            KeyDisplayName = keyDisplayName;
            KeyValue = keyValue;
            KeyDefaultValue = keyDefaultValue;
        }
    }
}