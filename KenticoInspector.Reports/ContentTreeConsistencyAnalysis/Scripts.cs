namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public static class Scripts
    {
        private static readonly string baseDirectory = $"{nameof(ContentTreeConsistencyAnalysis)}/Scripts";

        public static string GetCmsClass = $"{baseDirectory}/{nameof(GetCmsClass)}.sql";

        public static string GetCmsDocumentCoupledDataItems =
            $"{baseDirectory}/{nameof(GetCmsDocumentCoupledDataItems)}.sql";

        public static string GetDocumentIdsWithMissingTreeNode =
            $"{baseDirectory}/{nameof(GetDocumentIdsWithMissingTreeNode)}.sql";

        public static string GetDocumentNodeDetails = $"{baseDirectory}/{nameof(GetDocumentNodeDetails)}.sql";
        public static string GetTreeNodeDetails = $"{baseDirectory}/{nameof(GetTreeNodeDetails)}.sql";

        public static string GetTreeNodeIdsWithBadParentNodeId =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithBadParentNodeId)}.sql";

        public static string GetTreeNodeIdsWithBadParentSiteId =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithBadParentSiteId)}.sql";

        public static string GetTreeNodeIdsWithDuplicatedAliasPath =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithDuplicatedAliasPath)}.sql";

        public static string GetTreeNodeIdsWithLevelMismatchByAliasPath =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithLevelMismatchByAliasPath)}.sql";

        public static string GetTreeNodeIdsWithLevelMismatchByNodeLevel =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithLevelMismatchByNodeLevel)}.sql";

        public static string GetTreeNodeIdsWithMissingDocument =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithMissingDocument)}.sql";

        public static string GetTreeNodeIdsWithPageTypeNotAssignedToSite =
            $"{baseDirectory}/{nameof(GetTreeNodeIdsWithPageTypeNotAssignedToSite)}.sql";

        public static string GetLatestVersionHistoryIdForAllDocuments =
            $"{baseDirectory}/{nameof(GetLatestVersionHistoryIdForAllDocuments)}.sql";

        public static string GetVersionHistoryDetails = $"{baseDirectory}/{nameof(GetVersionHistoryDetails)}.sql";
    }
}