using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public IssueDescriptionsTerms IssueDescriptions { get; set; } = null!;

        public Term IssueTypes { get; set; } = null!;

        public Term WarningSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term IssueTypes { get; set; } = null!;

        public Term TemplateUsage { get; set; } = null!;

        public Term TransformationUsage { get; set; } = null!;

        public Term TransformationsWithIssues { get; set; } = null!;
    }

    public class IssueDescriptionsTerms
    {
        public Term ServerSideScript { get; set; } = null!;

        public Term DocumentsMacro { get; set; } = null!;

        public Term QueryMacro { get; set; } = null!;

        public Term XssQueryHelper { get; set; } = null!;

        public Term XssQueryString { get; set; } = null!;

        public Term XssHttpContext { get; set; } = null!;

        public Term XssServer { get; set; } = null!;

        public Term XssRequest { get; set; } = null!;

        public Term XssDocument { get; set; } = null!;

        public Term XssWindow { get; set; } = null!;
    }
}