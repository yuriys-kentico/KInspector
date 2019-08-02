using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.ClassTableValidation;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.Tests.Helpers;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ClassTableValidationTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public ClassTableValidationTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodStatus_When_DatabaseIsClean()
        {
            // Arrange
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<InformationSchemaTable>(Scripts.GetInformationSchemaTables))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClass))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        [Test]
        public void Should_ReturnErrorStatus_When_DatabaseHasClassWithNoTable()
        {
            // Arrange
            var tableResults = GetCleanTableResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<InformationSchemaTable>(Scripts.GetInformationSchemaTables))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            classResults.Add(new CmsClass
            {
                ClassDisplayName = "Has no table",
                ClassName = "HasNoTable",
                ClassTableName = "Custom_HasNoTable"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClass))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.GetAnonymousTableResult<TableResult<InformationSchemaTable>>("tableResults").Rows.Count() == 0);
            Assert.That(results.GetAnonymousTableResult<TableResult<CmsClass>>("classResults").Rows.Count() == 1);
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_DatabaseHasTableWithNoClass()
        {
            // Arrange
            var tableResults = GetCleanTableResults(false);
            tableResults.Add(new InformationSchemaTable
            {
                TableName = "HasNoClass"
            });

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<InformationSchemaTable>(Scripts.GetInformationSchemaTables))
                .Returns(tableResults);

            var classResults = GetCleanClassResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClass))
                .Returns(classResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.GetAnonymousTableResult<TableResult<InformationSchemaTable>>("tableResults").Rows.Count() == 1);
            Assert.That(results.GetAnonymousTableResult<TableResult<CmsClass>>("classResults").Rows.Count() == 0);
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        private List<CmsClass> GetCleanClassResults()
        {
            return new List<CmsClass>();
        }

        private List<InformationSchemaTable> GetCleanTableResults(bool includeWhitelistedTables = true)
        {
            var tableResults = new List<InformationSchemaTable>();
            if (includeWhitelistedTables && _mockInstanceDetails.DatabaseVersion.Major >= 10)
            {
                tableResults.Add(new InformationSchemaTable() { TableName = "CI_Migration" });
            }

            return tableResults;
        }
    }
}