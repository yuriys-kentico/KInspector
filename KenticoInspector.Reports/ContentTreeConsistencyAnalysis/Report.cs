using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health,
            ReportTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var treeNodeWithBadParentSiteResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithABadParentSite, Scripts.GetTreeNodeIdsWithBadParentSiteId);
            var treeNodeWithBadParentNodeResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithABadParentNode, Scripts.GetTreeNodeIdsWithBadParentNodeId);
            var treeNodeWithLevelInconsistencyAliasatPathTestResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithLevelInconsistencyAliasPath, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);
            var treeNodeWithLevelInconsistencyParentChildLevelTestResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithLevelInconsistencyParent, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);
            var treeNodeWithMissingDocumentResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithNoDocumentNode, Scripts.GetTreeNodeIdsWithMissingDocument);
            var treeNodeWithDuplicateAliasPathResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);
            var treeNodeWithPageTypeNotAssignedToSiteResults = GetTreeNodeTestResult(Metadata.Terms.TableNames.TreeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);
            var documentNodesWithMissingTreeNodeResults = GetDocumentNodeTestResult(Metadata.Terms.TableNames.DocumentNodesWithNoTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode);

            var workflowInconsistenciesResults = GetWorkflowInconsistencyResult();

            return CompileResults(
                treeNodeWithBadParentSiteResults,
                treeNodeWithBadParentNodeResults,
                treeNodeWithLevelInconsistencyAliasatPathTestResults,
                treeNodeWithLevelInconsistencyParentChildLevelTestResults,
                treeNodeWithMissingDocumentResults,
                treeNodeWithDuplicateAliasPathResults,
                treeNodeWithPageTypeNotAssignedToSiteResults,
                documentNodesWithMissingTreeNodeResults,
                workflowInconsistenciesResults
            );
        }

        private ConsistencyResult GetTreeNodeTestResult(string tableName, string sqlScriptRelativeFilePath)
        {
            return GetTestResult<CmsTreeNode>(tableName, sqlScriptRelativeFilePath, Scripts.GetTreeNodeDetails);
        }

        private ConsistencyResult GetDocumentNodeTestResult(string name, string script)
        {
            return GetTestResult<CmsDocument>(name, script, Scripts.GetDocumentNodeDetails);
        }

        private ConsistencyResult GetTestResult<T>(string tableName, string sqlScriptRelativeFilePath, string getDetailsScript)
        {
            var nodeIds = databaseService.ExecuteSqlFromFile<int>(sqlScriptRelativeFilePath);
            var details = databaseService.ExecuteSqlFromFile<T>(getDetailsScript, new { nodeIds });

            var status = details.Count() > 0 ? ReportResultsStatus.Error : ReportResultsStatus.Good;

            var data = new TableResult<T>
            {
                Name = tableName,
                Rows = details
            };

            return new ConsistencyResult(
                status,
                tableName,
                data
            );
        }

        private ConsistencyResult GetWorkflowInconsistencyResult()
        {
            var versionHistoryItems = GetVersionHistoryItems();

            var classItems = GetClasses(versionHistoryItems);

            // TODO: Find a use for this information
            // var allDocumentNodeIds = versionHistoryItems.Select(x => x.DocumentID);
            // var allDocumentNodes = _databaseService.ExecuteSqlFromFile<CmsDocumentNode>(Scripts.GetDocumentNodeDetails, new { IDs = allDocumentNodeIds.ToArray() });

            var comparisonResults = new List<VersionHistoryMismatchResult>();

            foreach (var cmsClass in classItems)
            {
                var classVersionHistoryItems = versionHistoryItems
                    .Where(versionHistoryItem => versionHistoryItem.VersionClassID == cmsClass.ClassID);

                var coupledDataIds = classVersionHistoryItems
                    .Select(classVersionHistoryItem => classVersionHistoryItem.CoupledDataID);

                var coupledData = GetCoupledData(cmsClass, coupledDataIds);

                var classComparisionResults = CompareVersionHistoryItemsWithPublishedItems(versionHistoryItems, coupledData, cmsClass.ClassFields);

                comparisonResults.AddRange(classComparisionResults);
            }

            var status = comparisonResults.Count() > 0 ? ReportResultsStatus.Error : ReportResultsStatus.Good;

            var tableName = Metadata.Terms.TableNames.WorkflowInconsistencies;

            var data = new TableResult<VersionHistoryMismatchResult>
            {
                Name = tableName,
                Rows = comparisonResults
            };

            return new ConsistencyResult(
                status,
                tableName,
                data
            );
        }

        private IEnumerable<CmsVersionHistoryItem> GetVersionHistoryItems()
        {
            var latestVersionHistoryIds = databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments);

            return databaseService.ExecuteSqlFromFile<CmsVersionHistoryItem>(Scripts.GetVersionHistoryDetails, new { latestVersionHistoryIds });
        }

        private IEnumerable<CmsClass> GetClasses(IEnumerable<CmsVersionHistoryItem> versionHistoryItems)
        {
            var classIds = versionHistoryItems
                .Select(versionHistoryItem => versionHistoryItem.VersionClassID);

            return databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClass, new { classIds });
        }

        private IEnumerable<IDictionary<string, object>> GetCoupledData(CmsClass cmsClass, IEnumerable<int> coupledDataIds)
        {
            var replacements = new Dictionary<string, string>
            {
                { "TableName", cmsClass.ClassTableName },
                { "IdColumnName", cmsClass.ClassIDColumn }
            };

            return databaseService.ExecuteSqlFromFileGeneric(Scripts.GetCmsDocumentCoupledDataItems, replacements, new { coupledDataIds });
        }

        private IEnumerable<VersionHistoryMismatchResult> CompareVersionHistoryItemsWithPublishedItems(IEnumerable<CmsVersionHistoryItem> versionHistoryItems, IEnumerable<IDictionary<string, object>> coupledData, IEnumerable<CmsClassField> classFields)
        {
            var issues = new List<VersionHistoryMismatchResult>();

            var idColumnName = classFields
                .FirstOrDefault(classField => classField.IsIdColumn)
                .Column;

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var coupledDataItem = coupledData
                    .FirstOrDefault(coupledDataRow => (int)coupledDataRow[idColumnName] == versionHistoryItem.CoupledDataID);

                if (coupledDataItem != null)
                {
                    foreach (var classField in classFields)
                    {
                        var historyVersionValueRaw = versionHistoryItem
                            .NodeXml
                            .Descendants(classField.Column)
                            .FirstOrDefault()?
                            .Value ?? classField.DefaultValue;

                        var coupledDataItemValue = coupledDataItem[classField.Column];

                        var columnName = classField.Caption ?? classField.Column;

                        var versionHistoryMismatchResult = new VersionHistoryMismatchResult(
                            versionHistoryItem.DocumentID,
                            columnName,
                            classField.ColumnType,
                            historyVersionValueRaw,
                            coupledDataItemValue
                        );

                        if (!versionHistoryMismatchResult.FieldValuesMatch)
                        {
                            issues.Add(versionHistoryMismatchResult);
                        }
                    }
                }
            }

            return issues;
        }

        private ReportResults CompileResults(params ConsistencyResult[] allTableResults)
        {
            var combinedResults = new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Good
            };

            IDictionary<string, object> dataAsDictionary = combinedResults.Data;

            foreach (var reportResult in allTableResults)
            {
                var tableName = reportResult.TableName;

                if (reportResult.Status == ReportResultsStatus.Error)
                {
                    dataAsDictionary.Add(tableName, reportResult.Data);

                    var count = reportResult.Data.Rows.Count;

                    combinedResults.Summary += Metadata.Terms.ErrorSummary.With(new { tableName, count });
                    combinedResults.Status = ReportResultsStatus.Error;
                }
            }

            if (combinedResults.Status == ReportResultsStatus.Good)
            {
                combinedResults.Summary = Metadata.Terms.GoodSummary;
            }

            return combinedResults;
        }
    }
}