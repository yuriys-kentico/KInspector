using System;
using System.Collections.Generic;
using System.Diagnostics;

using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// Represents a transformation used by a <see cref="WebPartProperty"/>.
    /// </summary>
    [DebuggerDisplay("{FullName} Issues:{Issues.Count}")]
    public class CmsTransformation
    {
        public int TransformationID { get; set; }

        public string TransformationName { get; set; } = null!;

        public string TransformationCode { get; set; } = null!;

        public string TransformationType { get; set; } = null!;

        public string ClassName { get; set; } = null!;

        public string FullName => $"{ClassName}.{TransformationName}";

        public IList<TransformationIssue> Issues { get; }

        public CmsTransformation()
        {
            Issues = new List<TransformationIssue>();
        }

        public void AddIssue(int snippetStartIndex, int snippetLength, string issueType, int snippetPadding = 5)
        {
            var startIndex = Math.Max(snippetStartIndex - snippetPadding, 0);
            var length = Math.Min(TransformationCode.Length - startIndex, snippetLength + snippetPadding * 2);

            Issues.Add(
                new TransformationIssue(TransformationCode.Substring(startIndex, length), issueType)
            );
        }
    }
}