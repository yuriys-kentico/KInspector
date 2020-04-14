using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Tags(Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            return CompileResults(
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithABadParentSite, Scripts.GetTreeNodeIdsWithBadParentSiteId),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithABadParentNode, Scripts.GetTreeNodeIdsWithBadParentNodeId),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithLevelInconsistencyAliasPath, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPath),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithLevelInconsistencyParent, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevel),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithNoDocumentNode, Scripts.GetTreeNodeIdsWithMissingDocument),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath),
                GetNodeResult<CmsTreeNode>(Metadata.Terms.TableNames.TreeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite),
                GetNodeResult<CmsDocument>(Metadata.Terms.TableNames.DocumentsWithNoTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode, Scripts.GetDocumentNodeDetails),
                GetWorkflowResult()
            );
        }

        private ConsistencyResult? GetNodeResult<T>(
            string tableName,
            string sqlScriptRelativeFilePath,
            string? getDetailsSqlRelativeFilePath = null
            ) where T : notnull
        {
            getDetailsSqlRelativeFilePath ??= Scripts.GetTreeNodeDetails;

            var nodeIds = databaseService.ExecuteSqlFromFile<int>(sqlScriptRelativeFilePath);
            var details = databaseService.ExecuteSqlFromFile<T>(getDetailsSqlRelativeFilePath, new { nodeIds });

            if (details.Any())
            {
                return new ConsistencyResult
                {
                    Data = details.AsResult().WithLabel(tableName),
                    Count = details.Count()
                };
            }

            return null;
        }

        private ConsistencyResult? GetWorkflowResult()
        {
            var versionHistoryItems = GetItemsWhereInManyIds<CmsVersionHistoryItem>(
                databaseService.ExecuteSqlFromFile<int>(Scripts.GetLatestVersionHistoryIdForAllDocuments),
                Scripts.GetVersionHistoryDetails
                );

            var classIds = versionHistoryItems
                .Select(versionHistoryItem => versionHistoryItem.VersionClassID)
                .Distinct();

            var classItems = GetItemsWhereInManyIds<CmsClass>(classIds, Scripts.GetCmsClass);

            // TODO: Find a use for this information
            // var allDocumentNodeIds = versionHistoryItems.Select(x => x.DocumentID);
            // var allDocumentNodes = _databaseService.ExecuteSqlFromFile<CmsDocumentNode>(Scripts.GetDocumentNodeDetails, new { IDs = allDocumentNodeIds.ToArray() });

            var comparisonResults = new List<VersionHistoryMismatchResult>();

            foreach (var cmsClass in classItems)
            {
                var classVersionHistoryItems = versionHistoryItems
                    .Where(versionHistoryItem => versionHistoryItem.VersionClassID == cmsClass.ClassID);

                var coupledDataIds = classVersionHistoryItems
                    .Where(versionHistoryItem => versionHistoryItem.CoupledDataID > 0)
                    .Select(versionHistoryItem => versionHistoryItem.CoupledDataID);

                if (coupledDataIds.Any() && cmsClass.ClassTableName != null && cmsClass.ClassIDColumn != null)
                {
                    var replacements = new Dictionary<string, string>
                    {
                        { "TableName", cmsClass.ClassTableName },
                        { "IdColumnName", cmsClass.ClassIDColumn }
                    };

                    var coupledData = databaseService.ExecuteSqlFromFile(Scripts.GetCmsDocumentCoupledDataItems, replacements, new { coupledDataIds });

                    comparisonResults.AddRange(CompareVersionHistoryItemsWithPublishedItems(classVersionHistoryItems, coupledData, cmsClass.ClassFields));
                }
            }

            if (comparisonResults.Any())
            {
                return new ConsistencyResult
                {
                    Data = comparisonResults.AsResult().WithLabel(Metadata.Terms.TableNames.WorkflowInconsistencies),
                    Count = comparisonResults.Count()
                };
            }

            return null;
        }

        private IEnumerable<T> GetItemsWhereInManyIds<T>(IEnumerable<int> manyIds, string sqlScriptRelativeFilePath)
        {
            const int maximumCountInParameters = 500;

            var idsBatches = manyIds
                .Select((id, index) => (id, index))
                .GroupBy(group => group.index / maximumCountInParameters, group => group.id);

            var items = new List<T>();

            foreach (var idsBatch in idsBatches)
            {
                items.AddRange(databaseService.ExecuteSqlFromFile<T>(sqlScriptRelativeFilePath, new { idsBatch }));
            }

            return items;
        }

        private static IEnumerable<VersionHistoryMismatchResult> CompareVersionHistoryItemsWithPublishedItems(
            IEnumerable<CmsVersionHistoryItem> versionHistoryItems,
            IEnumerable<IDictionary<string, object>> coupledData,
            IEnumerable<CmsClassField>? classFields)
        {
            var idColumnName = classFields
                .FirstOrDefault(classField => classField.IsIdColumn)
                .Column;

            foreach (var versionHistoryItem in versionHistoryItems)
            {
                var coupledDataItem = coupledData
                    .FirstOrDefault(coupledDataRow => (int)coupledDataRow[idColumnName] == versionHistoryItem.CoupledDataID);

                if (coupledDataItem != null && classFields != null)
                {
                    foreach (var classField in classFields)
                    {
                        var versionHistoryXmlValue = versionHistoryItem
                            .NodeXml?
                            .Descendants(classField.Column)
                            .FirstOrDefault()?
                            .Value ?? classField.DefaultValue;

                        var coupledDataColumnValue = coupledDataItem[classField.Column] ?? classField.DefaultValue;

                        var (versionHistoryValue, documentValue) = ProcessItemValues(classField.ColumnType, versionHistoryXmlValue, coupledDataColumnValue);

                        if (!(versionHistoryValue == documentValue))
                        {
                            yield return new VersionHistoryMismatchResult
                            {
                                DocumentId = versionHistoryItem.DocumentID,
                                DocumentName = versionHistoryItem.VersionDocumentName,
                                DocumentNamePath = versionHistoryItem.DocumentNamePath,
                                ColumnName = classField.Column,
                                Caption = classField.Caption,
                                DocumentValue = documentValue,
                                VersionHistoryValue = versionHistoryValue
                            };
                        }
                    }
                }
            }
        }

        private static (string? VersionHistoryValue, string? DocumentValue) ProcessItemValues(string columnType, string? versionHistoryXmlValue, object? coupledDataColumnValue)
        {
            if (!(versionHistoryXmlValue == null || coupledDataColumnValue == null))
            {
                switch (columnType)
                {
                    case FieldTypes.Date:
                    case FieldTypes.DateTime:
                        var versionHistoryValue = DateTimeOffset.Parse(versionHistoryXmlValue);
                        var documentValueAdjusted = new DateTimeOffset((DateTime)coupledDataColumnValue, versionHistoryValue.Offset);

                        return (versionHistoryValue.ToString(), documentValueAdjusted.ToString());

                    case FieldTypes.Boolean:
                        return (bool.Parse(versionHistoryXmlValue).ToString(), Convert.ToBoolean(coupledDataColumnValue).ToString());

                    case FieldTypes.Decimal:
                        var documentValue = Convert.ToDecimal(coupledDataColumnValue);
                        var documentValueString = documentValue.ToString();

                        var decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                        var position = documentValueString.IndexOf(decimalSeparator);
                        var precision = (position == -1) ? 0 : documentValueString.Length - position - 1;

                        var formatting = "0.";
                        for (int i = 0; i < precision; i++)
                        {
                            formatting += "0";
                        }

                        return (decimal.Parse(versionHistoryXmlValue).ToString(formatting), documentValueString);

                    default:
                        return (Regex.Replace(versionHistoryXmlValue, @"\n", "\r\n"), coupledDataColumnValue.ToString());
                }
            }

            return (versionHistoryXmlValue, coupledDataColumnValue?.ToString());
        }

        private ReportResults CompileResults(params ConsistencyResult?[] allResults)
        {
            var errorResults = allResults.Where(result => result != null);

            if (!errorResults.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var results = new ReportResults(ResultsStatus.Error);

            foreach (var reportResult in errorResults)
            {
                if (reportResult != null)
                {
                    results.Data.Add(reportResult.Data);

                    var tableName = reportResult.Data.Label;
                    var count = reportResult.Count;

                    results.Summary += Metadata.Terms.ErrorSummary.With(new { tableName, count });
                }
            }

            return results;
        }
    }
}