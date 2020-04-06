using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TransformationResult : DynamicObject
    {
        private readonly IDictionary<string, string> dynamicIssueProperties = new Dictionary<string, string>();

        [JsonProperty]
        public int TransformationID { get; set; }

        [JsonProperty]
        public string? TransformationFullName { get; set; }

        [JsonProperty]
        public string? TransformationType { get; set; }

        [JsonProperty]
        public int TransformationUses { get; set; }

        public TransformationResult(CmsTransformation? transformation, int uses, IEnumerable<string> detectedIssueTypes)
        {
            if (transformation != null)
            {
                TransformationID = transformation.TransformationID;
                TransformationFullName = transformation.FullName;
                TransformationType = transformation.TransformationType;

                foreach (var issueType in detectedIssueTypes)
                {
                    dynamicIssueProperties.TryAdd(issueType, string.Empty);
                }

                var groupedIssues = transformation.Issues
                    .GroupBy(issue => issue.IssueType);

                foreach (var issueGroup in groupedIssues)
                {
                    foreach (var issue in issueGroup)
                    {
                        dynamicIssueProperties[issueGroup.Key] += HttpUtility.HtmlEncode($"...{issue.CodeSnippet}...");
                    }
                }
            }
            TransformationUses = uses;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var exists = dynamicIssueProperties.TryGetValue(binder.Name, out string? value);

            result = value;

            return exists;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dynamicIssueProperties.Keys;
        }
    }
}