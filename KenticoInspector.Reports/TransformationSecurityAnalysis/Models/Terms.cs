using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; }

        public IssueDescriptionsTerms IssueDescriptions { get; set; }

        public Term IssueTypes { get; set; }

        public Term WarningSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }
    }

    public class TableNamesTerms
    {
        public Term IssueTypes { get; set; }

        public Term TemplateUsage { get; set; }

        public Term TransformationUsage { get; set; }

        public Term TransformationsWithIssues { get; set; }
    }

    public class IssueDescriptionsTerms
    {
        public Term ServerSideScript { get; set; }

        public Term DocumentsMacro { get; set; }

        public Term QueryMacro { get; set; }

        public Term XssQueryHelper { get; set; }

        public Term XssQueryString { get; set; }

        public Term XssHttpContext { get; set; }

        public Term XssServer { get; set; }

        public Term XssRequest { get; set; }

        public Term XssDocument { get; set; }

        public Term XssWindow { get; set; }
    }
}