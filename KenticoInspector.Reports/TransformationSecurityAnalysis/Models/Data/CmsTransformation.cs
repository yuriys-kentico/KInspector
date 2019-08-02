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
        public string TransformationName { get; set; }

        public string TransformationCode { get; set; }

        public TransformationType TransformationType { get; set; }

        public string ClassName { get; set; }

        public string FullName => $"{ClassName}.{TransformationName}";

        public IList<TransformationIssue> Issues { get; } = new List<TransformationIssue>();

        public void AddIssue(int snippetStartIndex, int snippetLength, string issueType)
        {
            var startIndex = Math.Max(snippetStartIndex - TransformationIssue.SnippetPadding, 0);
            var length = Math.Min(TransformationCode.Length - startIndex, snippetLength + TransformationIssue.SnippetPadding * 2);

            Issues.Add(
                new TransformationIssue(TransformationCode.Substring(startIndex, length), issueType)
            );
        }
    }
}