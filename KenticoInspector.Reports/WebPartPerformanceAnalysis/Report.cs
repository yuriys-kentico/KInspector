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

        public Report(IDatabaseService databaseService, IModuleMetadataService reportMetadataService) : base(reportMetadataService)
        {
            _databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Performance,
            ReportTags.PortalEngine
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

                if (!treeNodesUsingPageTemplate.Any())
                {
                    continue;
                }

                var webPartsWithIssues = ExtractWebPartsWithEmptyColumnsProperty(pageTemplate, treeNodesUsingPageTemplate);

                if (!webPartsWithIssues.Any())
                {
                    continue;
                }

                results.Add(new TemplateAnalysisResult
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
                .PageTemplateWebParts?
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
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var webPartsWithIssues = templateAnalysisResults
                .SelectMany(x => x.WebPartsWithIssues);

            var treeNodesWithIssues = templateAnalysisResults
                .SelectMany(x => x.TreeNodesWithIssues);

            var affectedDocumentCount = treeNodesWithIssues.Count();
            var affectedTemplateCount = templateAnalysisResults.Count();
            var affectedWebPartCount = webPartsWithIssues.Count();

            return new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { affectedDocumentCount, affectedTemplateCount, affectedWebPartCount }),
                Data =
                {
                    templateAnalysisResults.AsResult().WithLabel(Metadata.Terms.TableNames.TemplatesWithIssues),
                    webPartsWithIssues.AsResult().WithLabel(Metadata.Terms.TableNames.WebPartsWithIssues),
                    treeNodesWithIssues.AsResult().WithLabel(Metadata.Terms.TableNames.TreeNodesWithIssues)
                }
            };
        }
    }
}