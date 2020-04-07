using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.ClassTableValidation;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ClassTableValidationTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private List<CmsClass> CmsClassesWithTables => new List<CmsClass>();

        public ClassTableValidationTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object, mockInstanceService.Object));
        }

        [TestCase(Category = "No invalid classes or tables", TestName = "Database without invalid classes or tables produces a good result")]
        public void Should_ReturnGoodResult_When_DatabaseWithoutIssues()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [TestCase(Category = "Class with no table", TestName = "Database with a class with no table produces an error result and lists one class")]
        public void Should_ReturnErrorResult_When_DatabaseWithClassWithNoTable()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues();

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            classResults.Add(new CmsClass
            {
                ClassDisplayName = "Has no table",
                ClassName = "HasNoTable",
                ClassTableName = "Custom_HasNoTable"
            });

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsClass>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
        }

        [TestCase(Category = "Table with no class", TestName = "Database with a table with no class produces an error result and lists one table")]
        public void Should_ReturnErrorResult_When_DatabaseWithTableWithNoClass()
        {
            // Arrange
            var tableResults = GetTableResultsWithoutIssues(false);
            tableResults.Add(new DatabaseTable
            {
                TableName = "HasNoClass"
            });

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTable>(Scripts.GetTablesWithMissingClass))
                .Returns(tableResults);

            var classResults = CmsClassesWithTables;

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable))
                .Returns(classResults);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<DatabaseTable>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
        }

        private List<DatabaseTable> GetTableResultsWithoutIssues(bool includeWhitelistedTables = true)
        {
            var tableResults = new List<DatabaseTable>();

            if (includeWhitelistedTables && mockInstanceDetails.DatabaseVersion.Major >= 10)
            {
                tableResults.Add(new DatabaseTable()
                {
                    TableName = "CI_Migration"
                });
            }

            return tableResults;
        }
    }
}