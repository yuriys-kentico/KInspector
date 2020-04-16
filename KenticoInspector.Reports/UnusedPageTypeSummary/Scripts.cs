namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(UnusedPageTypeSummary)}/Scripts";

        public static string GetCmsClassNotInViewCmsTreeJoined =
            $"{baseDirectory}/{nameof(GetCmsClassNotInViewCmsTreeJoined)}.sql";
    }
}