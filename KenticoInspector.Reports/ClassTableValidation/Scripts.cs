namespace KenticoInspector.Reports.ClassTableValidation
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(ClassTableValidation)}/Scripts";

        public static string GetCmsClass = $"{BaseDirectory}/{nameof(GetCmsClass)}.sql";
        public static string GetInformationSchemaTables = $"{BaseDirectory}/{nameof(GetInformationSchemaTables)}.sql";
    }
}