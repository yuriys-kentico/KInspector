using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary.Models;
using KenticoInspector.Reports.Tests.Helpers;

using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class RobotsTxtConfigurationSummaryTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public RobotsTxtConfigurationSummaryTests(int majorVersion) : base(majorVersion)
        {
        }

        [Test]
        public void Should_ReturnGoodResult_When_RobotsTxtFound()
        {
            // Arrange
            _mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockInstanceService.Object.CurrentInstance;

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            var baseUri = new Uri(mockInstance.Url);

            var expectedUri = new Uri(baseUri, DefaultKenticoPaths.RobotsTxtRelative);

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnGoodResult_When_SiteIsInSubDirectoryAndRobotsTxtFound()
        {
            // Arrange

            _mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = _mockInstanceService.Object.CurrentInstance;

            var baseUrl = mockInstance.Url;
            mockInstance.Url += "/subdirectory";

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            var expectedUri = new Uri($"{baseUrl}/{DefaultKenticoPaths.RobotsTxtRelative}");

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnWarningResult_When_RobotsTxtNotFound()
        {
            // Arrange
            _mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.NotFound, out Mock<HttpMessageHandler> mockHttpMessageHandler);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        private Report ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode httpStatusCode, out Mock<HttpMessageHandler> mockHttpMessageHandler)
        {
            mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = httpStatusCode })
                .Verifiable();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var report = new Report(_mockInstanceService.Object, _mockReportMetadataService.Object, httpClient);

            MockReportMetadataServiceHelper.SetupReportMetadataService<Terms>(_mockReportMetadataService, report);

            return report;
        }

        private static void AssertUrlCalled(Mock<HttpMessageHandler> handlerMock, Uri expectedUri)
        {
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}