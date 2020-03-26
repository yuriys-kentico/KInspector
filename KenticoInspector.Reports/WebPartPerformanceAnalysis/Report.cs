using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models;
using KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            _databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.PortalEngine,
            ReportTags.Performance,
            ReportTags.WebParts
        };

        public override ReportResults GetResults()
        {
            var pageTemplatesWithWebPartsWithColumnsProperty = _databaseService.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplatesWithWebPartsWithColumnsProperty);

            var pageTemplatesWithWebPartsWithColumnsPropertyIds = pageTemplatesWithWebPartsWithColumnsProperty
                .Select(pageTemplate => pageTemplate.PageTemplateID);

            var treeNodesUsingPageTemplates = _databaseService.ExecuteSqlFromFile<CmsTreeNode>(Scripts.GetTreeNodesUsingPageTemplates, new { pageTemplatesWithWebPartsWithColumnsPropertyIds });

            var templateAnalysisResults = GetTemplateAnalysisResults(pageTemplatesWithWebPartsWithColumnsProperty, treeNodesUsingPageTemplates);

            return CompileResults(templateAnalysisResults);
        }

        private IEnumerable<TemplateAnalysisResult> GetTemplateAnalysisResults(IEnumerable<CmsPageTemplate> pageTemplates, IEnumerable<CmsTreeNode> treeNodesUsingPageTemplates)
        {
            var results = new List<TemplateAnalysisResult>();

            foreach (var pageTemplate in pageTemplates)
            {
                var treeNodesUsingPageTemplate = treeNodesUsingPageTemplates
                    .Where(treeNode => treeNode.DocumentPageTemplateID == pageTemplate.PageTemplateID);

                var webPartsWithIssues = ExtractWebPartsWithEmptyColumnsProperty(pageTemplate, treeNodesUsingPageTemplate);

                if (!webPartsWithIssues.Any())
                {
                    continue;
                }

                results.Add(new TemplateAnalysisResult()
                {
                    PageTemplateID = pageTemplate.PageTemplateID,
                    PageTemplateDisplayName = pageTemplate.PageTemplateDisplayName,
                    PageTemplateCodeName = pageTemplate.PageTemplateCodeName,
                    TreeNodesWithIssues = treeNodesUsingPageTemplate,
                    WebPartsWithIssues = webPartsWithIssues
                });
            }

            return results;
        }

        private IEnumerable<WebPartAnalysisResult> ExtractWebPartsWithEmptyColumnsProperty(CmsPageTemplate template, IEnumerable<CmsTreeNode> treeNodes)
        {
            var emptyColumnsWebPartProperties = template
                .PageTemplateWebParts
                .Descendants("property")
                .Where(property => property
                    .Attribute("name")
                    .Value == "columns"
                )
                .Where(property => string.IsNullOrWhiteSpace(property.Value));

            var webPartXmls = emptyColumnsWebPartProperties.Ancestors("webpart");

            foreach (var webPartXml in webPartXmls)
            {
                yield return new WebPartAnalysisResult
                {
                    WebPartControlId = webPartXml
                        .Attribute("controlid")
                        .Value,
                    WebPartName = webPartXml
                        .Elements("property")
                        .FirstOrDefault(p => p.Name == "webparttitle")?
                        .Value,
                    WebPartType = webPartXml
                        .Attribute("type")
                        .Value,
                    PageTemplateId = template.PageTemplateID,
                    TreeNodes = treeNodes
                };
            }
        }

        private ReportResults CompileResults(IEnumerable<TemplateAnalysisResult> templateAnalysisResults)
        {
            if (!templateAnalysisResults.Any())
            {
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var templateAnalysisResultsResult = templateAnalysisResults.AsResult().WithLabel(Metadata.Terms.TableNames.TemplatesWithIssues);

            var webPartsWithIssues = templateAnalysisResults
                .SelectMany(x => x.WebPartsWithIssues);

            var webPartAnalysisResultsResult = webPartsWithIssues.AsResult().WithLabel(Metadata.Terms.TableNames.WebPartsWithIssues);

            var treeNodesWithIssues = templateAnalysisResults
                .SelectMany(x => x.TreeNodesWithIssues);

            var TreeNodesWithIssuesResult = treeNodesWithIssues.AsResult().WithLabel(Metadata.Terms.TableNames.TreeNodesWithIssues);

            var affectedDocumentCount = treeNodesWithIssues.Count();
            var affectedTemplateCount = templateAnalysisResults.Count();
            var affectedWebPartCount = webPartsWithIssues.Count();

            return new ReportResults(ReportResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { affectedDocumentCount, affectedTemplateCount, affectedWebPartCount }),
                Data =
                {
                    templateAnalysisResultsResult,
                    webPartAnalysisResultsResult,
                    TreeNodesWithIssuesResult
                },
            };
        }
    }
}