using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.ColumnFieldValidation;
using KenticoInspector.Reports.ColumnFieldValidation.Models;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Data;
using KenticoInspector.Reports.ColumnFieldValidation.Models.Results;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;
using System.Xml.Linq;
using System.IO;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ColumnFieldValidationTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsClass> ValidCmsClasses => new List<CmsClass>
        {
            new CmsClass
            {
                ClassName = "Class.1",
                ClassTableName = "Class1",
                ClassXmlSchema = GetXDocumentFromFile(@"TestData\CMS_Class\Class1\ClassXmlSchema.xml")
            }
        };

        private IEnumerable<CmsClass> InvalidCmsClasses => new List<CmsClass>
        {
            new CmsClass
            {
                ClassName = "Class.1",
                ClassTableName = "Class1",
                ClassXmlSchema = GetXDocumentFromFile(@"TestData\CMS_Class\Class1\ClassXmlSchemaWithAddedField.xml")
            }
        };

        private IEnumerable<TableColumn> ValidTableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column1",
                Data_Type = "int"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column2",
                Data_Type = "bigint"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column3",
                Data_Type = "float"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column4",
                Data_Type = "decimal"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column5",
                Data_Type = "nvarchar"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column6",
                Data_Type = "datetime2"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column7",
                Data_Type = "bit"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column8",
                Data_Type = "varbinary"
            },
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column9",
                Data_Type = "uniqueidentifier"
            }
        };

        private IEnumerable<TableColumn> InvalidTableColumns => new List<TableColumn>(ValidTableColumns)
        {
            new TableColumn
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column10",
                Data_Type = "bit"
            }
        };

        public ColumnFieldValidationTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [TestCase(
            Category = "Class and table match",
            TestName = "Matching class and table produce a good result"
            )]
        public void Should_ReturnGoodResult_When_ClassAndTableMatch()
        {
            // Arrange
            ArrangeDatabaseService(
                ValidCmsClasses,
                ValidTableColumns
                );

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );

            Assert.That(
                results.Summary,
                Is.EqualTo(mockReport.Metadata.Terms.Summaries.Good.ToString())
                );
        }

        [TestCase(
            Category = "Class has added field",
            TestName = "Class with added field produces an error result"
            )]
        public void Should_ReturnErrorResult_When_ClassHasAddedField()
        {
            // Arrange
            ArrangeDatabaseService(
                InvalidCmsClasses,
                ValidTableColumns
                );

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );

            Assert.That(
                results.Data.First<TableResult<CmsClassResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );
        }

        [TestCase(
            Category = "Table has added column",
            TestName = "Table with added column produces an error result"
            )]
        public void Should_ReturnErrorResult_When_TableHasAddedColumn()
        {
            // Arrange
            ArrangeDatabaseService(
                ValidCmsClasses,
                InvalidTableColumns
                );

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );

            Assert.That(
                results.Data.First<TableResult<TableResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );
        }

        private void ArrangeDatabaseService(
            IEnumerable<CmsClass> cmsClasses,
            IEnumerable<TableColumn> tableColumns
            )
        {
            mockDatabaseService.SetupExecuteSqlFromFile(
                Scripts.GetCmsClasses,
                cmsClasses
                );

            var classTableNames = cmsClasses
                .Select(cmsClass => cmsClass.ClassTableName);

            mockDatabaseService.SetupExecuteSqlFromFile(
                Scripts.GetTableColumns,
                nameof(classTableNames),
                classTableNames,
                tableColumns
                );
        }

        private static XDocument GetXDocumentFromFile(string path) => XDocument.Parse(File.ReadAllText(path));
    }
}