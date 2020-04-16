namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis
{
    public class TransformationIssue
    {
        public string CodeSnippet { get; }

        public string IssueType { get; }

        public TransformationIssue(
            string codeSnippet,
            string issueType
            )
        {
            CodeSnippet = codeSnippet;
            IssueType = issueType;
        }
    }
}