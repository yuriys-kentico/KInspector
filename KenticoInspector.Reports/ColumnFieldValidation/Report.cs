﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.ColumnFieldValidation.Models;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Data;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Results;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.ColumnFieldValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(
            IDatabaseService databaseService
            )
        {
            this.databaseService = databaseService;
        }

        [Tags(Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var cmsClasses = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClasses);

            var classTableNames = cmsClasses
                .Select(cmsClass => cmsClass.ClassTableName);

            var tableColumns = databaseService.ExecuteSqlFromFile<TableColumn>(
                Scripts.GetTableColumns,
                new
                {
                    classTableNames
                }
                );

            var cmsClassesWithAddedFields = GetCmsClassesWithAddedFields(
                cmsClasses,
                tableColumns
                );

            var tablesWithAddedColumns = GetTablesWithAddedColumns(
                tableColumns,
                cmsClasses
                );

            return CompileResults(
                cmsClassesWithAddedFields,
                tablesWithAddedColumns
                );
        }

        private IEnumerable<CmsClassResult> GetCmsClassesWithAddedFields(
            IEnumerable<CmsClass> cmsClasses,
            IEnumerable<TableColumn> tableColumns
            )
        {
            foreach (var cmsClass in cmsClasses)
            {
                var classFieldNameTypes = cmsClass.ClassXmlSchema
                    .Descendants()
                    .Where(element => element.Name.LocalName == "element")
                    .Where(
                        element => element.Attribute("name")
                            .Value != "NewDataSet"
                        )
                    .Where(
                        element => element.Attribute("name")
                            .Value != cmsClass.ClassTableName
                        )
                    .Select(GetClassFieldNameType);

                var tableColumnNameTypes = tableColumns
                    .Where(
                        tableColumn => tableColumn.Table_Name.Equals(
                            cmsClass.ClassTableName,
                            StringComparison.InvariantCultureIgnoreCase
                            )
                        );

                var addedFields = classFieldNameTypes
                    .Where(
                        classFieldNameType => !tableColumnNameTypes
                            .Any(
                                tableColumnNameType => classFieldNameType.Name == tableColumnNameType.Column_Name
                                    && tableColumnNameType.Data_Type != null
                                    && classFieldNameType.Type.StartsWith(
                                        tableColumnNameType
                                            .Data_Type
                                        )
                                )
                        );

                if (addedFields.Any())
                    yield return new CmsClassResult
                    {
                        ClassDisplayName = cmsClass.ClassDisplayName,
                        ClassFieldsNotInTable = string.Join(
                            ", ",
                            addedFields
                            ),
                        ClassID = cmsClass.ClassID,
                        ClassName = cmsClass.ClassName,
                        ClassTableName = cmsClass.ClassTableName
                    };
            }
        }

        private IEnumerable<TableResult> GetTablesWithAddedColumns(
            IEnumerable<TableColumn> tableColumns,
            IEnumerable<CmsClass> cmsClasses
            )
        {
            var tableColumnGroups = tableColumns
                .GroupBy(tableColumn => tableColumn.Table_Name);

            foreach (var tableColumnGroup in tableColumnGroups)
            {
                var matchingCmsClass = cmsClasses
                    .First(
                        cmsClass => cmsClass.ClassTableName != null
                            && cmsClass.ClassTableName.Equals(
                                tableColumnGroup.Key,
                                StringComparison.InvariantCultureIgnoreCase
                                )
                        );

                var classFields = matchingCmsClass.ClassXmlSchema
                    .Descendants()
                    .Where(element => element.Name.LocalName == "element")
                    .Where(
                        element => element.Attribute("name")
                            .Value != "NewDataSet"
                        )
                    .Where(
                        element => element.Attribute("name")
                            .Value != matchingCmsClass.ClassTableName
                        )
                    .Select(GetClassFieldNameType);

                var addedColumns = tableColumnGroup
                    .Where(
                        tableColumn => !classFields
                            .Any(
                                classField => classField.Name == tableColumn.Column_Name
                                    && tableColumn.Data_Type != null
                                    && classField.Type.StartsWith(tableColumn.Data_Type)
                                )
                        )
                    .Select(column => (column.Column_Name, column.Data_Type));

                if (addedColumns.Any())
                    yield return new TableResult
                    {
                        TableColumnsNotInClass = string.Join(
                            ", ",
                            addedColumns
                            ),
                        TableName = tableColumnGroup.Key
                    };
            }
        }

        private (string Name, string Type) GetClassFieldNameType(XElement element)
        {
            var name = element.Attribute("name")
                .Value;

            var type = "(complex type)";

            if (element.Attribute("type") != null)
                type = element.Attribute("type")
                    .Value;

            var attributeDataType = element
                .Attributes()
                .FirstOrDefault(attribute => attribute.Name.LocalName == "DataType");

            if (attributeDataType != null) type = attributeDataType.Value;

            var childElementWithType = element
                .Descendants()
                .FirstOrDefault(childElement => childElement.Name.LocalName == "restriction");

            if (childElementWithType?.Attribute("base") != null)
                type = childElementWithType.Attribute("base")
                    .Value;

            switch (type)
            {
                case "xs:int":
                    type = "int";

                    break;

                case "xs:long":
                    type = "bigint";

                    break;

                case "xs:double":
                    type = "float";

                    break;

                case "xs:decimal":
                    type = "decimal";

                    break;

                case "xs:string":
                    type = "nvarchar";

                    break;

                case "xs:dateTime":
                    type = "datetime2";

                    break;

                case "xs:boolean":
                    type = "bit";

                    break;

                case "xs:base64Binary":
                    type = "varbinary";

                    break;

                case "System.Guid, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                case "System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089":
                    type = "uniqueidentifier";

                    break;
            }

            return (name, type);
        }

        private ReportResults CompileResults(
            IEnumerable<CmsClassResult> cmsClassesWithAddedFields,
            IEnumerable<TableResult> tablesWithAddedColumns
            )
        {
            if (!cmsClassesWithAddedFields.Any() && !tablesWithAddedColumns.Any())
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };

            var cmsClassesResultCount = cmsClassesWithAddedFields.Count();
            var tablesResultCount = tablesWithAddedColumns.Count();

            return new ReportResults(ResultsStatus.Error)
            {
                Summary = Metadata.Terms.Summaries.Error.With(
                    new
                    {
                        cmsClassesResultCount,
                        tablesResultCount
                    }
                    ),
                Data =
                {
                    cmsClassesWithAddedFields.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.ClassesWithAddedFields),
                    tablesWithAddedColumns.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.TablesWithAddedColumns)
                }
            };
        }
    }
}