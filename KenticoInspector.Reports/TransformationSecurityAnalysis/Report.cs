using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Analysis;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override IList<string> Tags => new List<string>
        {
            ReportTags.PortalEngine,
            ReportTags.Health,
            ReportTags.Security
        };

        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var transformations = databaseService.ExecuteSqlFromFile<CmsTransformation>(Scripts.GetTransformations);

            var transformationsWithIssues = GetTransformationsWithIssues(transformations);

            var treeNodes = databaseService.ExecuteSqlFromFile<CmsTreeNode>(Scripts.GetTreeNodes);

            var DocumentPageTemplateIDs = treeNodes
                .Select(treeNode => treeNode.DocumentPageTemplateID);

            var pageTemplates = databaseService.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetPageTemplates, new { DocumentPageTemplateIDs });

            foreach (var pageTemplate in pageTemplates)
            {
                pageTemplate.TreeNodes = treeNodes
                    .Where(page => page.DocumentPageTemplateID == pageTemplate.PageTemplateID);
            }

            var pageTemplatesUsingTransformationsWithIssues = GetPageTemplatesUsingTransformationsWithIssues(pageTemplates, transformationsWithIssues);

            var sites = instanceService
                .GetInstanceDetails(instanceService.CurrentInstance)
                .Sites;

            return CompileResults(pageTemplatesUsingTransformationsWithIssues, sites);
        }

        private IEnumerable<CmsTransformation> GetTransformationsWithIssues(IEnumerable<CmsTransformation> transformations)
        {
            foreach (var transformation in transformations)
            {
                AnalyzeTransformation(transformation);
            }

            return transformations
                .Where(transformation => transformation
                    .Issues
                    .Any()
                );
        }

        private void AnalyzeTransformation(CmsTransformation transformation)
        {
            var issueAnalyzersObject = new IssueAnalyzers(Metadata.Terms);

            var issueAnalyzerPublicInstanceMethods = issueAnalyzersObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.ReturnType == typeof(void));

            foreach (var issueAnalyzerPublicInstanceMethod in issueAnalyzerPublicInstanceMethods)
            {
                issueAnalyzerPublicInstanceMethod.Invoke(issueAnalyzersObject, new object[] { transformation });
            }
        }

        private static IEnumerable<CmsPageTemplate> GetPageTemplatesUsingTransformationsWithIssues(IEnumerable<CmsPageTemplate> pageTemplates, IEnumerable<CmsTransformation> transformationsWithIssues)
        {
            foreach (var pageTemplate in pageTemplates)
            {
                if (pageTemplate.WebParts != null)
                {
                    foreach (var webPart in pageTemplate.WebParts)
                    {
                        foreach (var webPartProperty in webPart.Properties)
                        {
                            var matchingTransformation = transformationsWithIssues
                                .SingleOrDefault(transformation => transformation.FullName == webPartProperty.TransformationFullName);

                            if (matchingTransformation != null)
                            {
                                webPartProperty.Transformation = matchingTransformation;
                            }
                        }

                        webPart.RemovePropertiesWithoutTransformations();
                    }
                }

                pageTemplate.RemoveWebPartsWithNoProperties();
            }

            return pageTemplates
                .Where(pageTemplate => pageTemplate
                    .WebParts
                    .Any()
                );
        }

        private ReportResults CompileResults(IEnumerable<CmsPageTemplate> pageTemplates, IEnumerable<CmsSite> sites)
        {
            var allIssues = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.WebParts)
                .SelectMany(webPart => webPart.Properties)
                .SelectMany(webPartProperty => webPartProperty.Transformation?.Issues);

            if (!allIssues.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var oneIssueOfEachType = allIssues
                .GroupBy(transformationIssue => transformationIssue.IssueType)
                .Select(g => g.First());

            var issueTypes = oneIssueOfEachType
                .Select(AsIssueTypeResult);

            var issueTypesResult = issueTypes.AsResult().WithLabel(Metadata.Terms.TableNames.IssueTypes);

            var usedIssueTypes = IssueAnalyzers.DetectedIssueTypes
                .Keys
                .Where(issueType => oneIssueOfEachType
                    .Select(issue => issue.IssueType)
                    .Contains(issueType)
                );

            var allTransformations = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.WebParts)
                .SelectMany(webPart => webPart.Properties)
                .Select(webPartProperty => webPartProperty.Transformation)
                .GroupBy(transformation => transformation?.FullName)
                .Select(g => g.First());

            var transformationsResultRows = allTransformations
                .Select(transformation => new TransformationResult(transformation, CountTransformationUses(transformation, pageTemplates), usedIssueTypes));

            var transformationsResult = transformationsResultRows.AsResult().WithLabel(Metadata.Terms.TableNames.TransformationsWithIssues);

            var transformationUsageResultRows = pageTemplates
                .SelectMany(AsTransformationUsageResults);

            var transformationUsageResult = transformationUsageResultRows.AsResult().WithLabel(Metadata.Terms.TableNames.TransformationUsage);

            var templateUsageResultRows = pageTemplates
                .SelectMany(pageTemplate => pageTemplate.TreeNodes)
                .Select(page => new TemplateUsageResult(page, sites));

            var templateUsageResult = templateUsageResultRows.AsResult().WithLabel(Metadata.Terms.TableNames.TemplateUsage);

            var summaryCount = allTransformations
                .Select(transformation => transformation?.Issues)
                .Count();

            var issueTypesAsCsv = string.Join(',', usedIssueTypes
                .Select(issueType => Metadata.Terms.IssueTypes.With(new { issueType })));

            return new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { summaryCount, issueTypesAsCsv }),
                Data =
                {
                    issueTypesResult,
                    transformationsResult,
                    transformationUsageResult,
                    templateUsageResult
                }
            };
        }

        private IssueTypeResult AsIssueTypeResult(TransformationIssue transformationIssue)
        {
            var issueType = transformationIssue.IssueType;

            string? description = null;

            if (IssueAnalyzers.DetectedIssueTypes.TryGetValue(issueType, out Term? descriptionTerm))
            {
                description = descriptionTerm;
            }

            return new IssueTypeResult()
            {
                Name = Metadata.Terms.IssueTypes.With(new { issueType }),
                Description = description
            };
        }

        private static int CountTransformationUses(CmsTransformation? transformation, IEnumerable<CmsPageTemplate> pageTemplates)
        {
            var totalCount = 0;

            foreach (var pageTemplate in pageTemplates)
            {
                var templateCount = 0;

                if (pageTemplate.WebParts != null)
                {
                    foreach (var webPart in pageTemplate.WebParts)
                    {
                        foreach (var property in webPart.Properties)
                        {
                            if (property.Transformation?.FullName == transformation?.FullName)
                            {
                                templateCount++;
                            }
                        }
                    }
                }

                if (templateCount > 0)
                {
                    templateCount *= pageTemplate.TreeNodes.Count();
                }

                totalCount += templateCount;
            }

            return totalCount;
        }

        private static IEnumerable<TransformationUsageResult> AsTransformationUsageResults(CmsPageTemplate pageTemplate)
        {
            if (pageTemplate.WebParts != null)
            {
                foreach (var webPart in pageTemplate.WebParts)
                {
                    foreach (var property in webPart.Properties)
                    {
                        if (property.Transformation != null)
                        {
                            yield return new TransformationUsageResult
                            {
                                PageTemplateID = pageTemplate.PageTemplateID,
                                PageTemplateCodeName = pageTemplate.PageTemplateCodeName,
                                PageTemplateDisplayName = pageTemplate.PageTemplateDisplayName,
                                PageTemplateWebParts = pageTemplate.PageTemplateWebParts,

                                WebPartControlId = webPart.ControlId,
                                WebPartPropertyName = property.Name,

                                TransformationID = property.Transformation.TransformationID,
                                TransformationFullName = property.Transformation.FullName
                            };
                        }
                    }
                }
            }
        }
    }
}