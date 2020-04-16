namespace KenticoInspector.Reports.TransformationSecurityAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(TransformationSecurityAnalysis)}/Scripts";
        public static string GetTransformations = $"{baseDirectory}/{nameof(GetTransformations)}.sql";
        public static string GetPageTemplates = $"{baseDirectory}/{nameof(GetPageTemplates)}.sql";
        public static string GetTreeNodes = $"{baseDirectory}/{nameof(GetTreeNodes)}.sql";
    }
}