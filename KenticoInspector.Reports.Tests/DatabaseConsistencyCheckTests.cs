﻿using System.Data;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.DatabaseConsistencyCheck;
using KenticoInspector.Reports.DatabaseConsistencyCheck.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseConsistencyCheckTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public DatabaseConsistencyCheckTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_ResultsWithoutIssues()
        {
            // Arrange
            var emptyResult = new DataTable();
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(emptyResult);
#pragma warning restore 0618
            // Act
            var results = _mockReport.GetResults();

            //Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnErrorResult_When_ResultsWithIssues()
        {
            // Arrange
            var result = new DataTable();
            result.Columns.Add("TestColumn");
            result.Rows.Add("value");

# pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(result);
# pragma warning restore 0618

            // Act
            var results = _mockReport.GetResults();

            //Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Error));
        }
    }
}