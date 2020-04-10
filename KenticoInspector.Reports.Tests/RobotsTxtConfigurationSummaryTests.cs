using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary.Models;

using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class RobotsTxtConfigurationSummaryTests : AbstractReportTests<Report, Terms>
    {
        private Report? mockReport;

        public RobotsTxtConfigurationSummaryTests(int majorVersion) : base(majorVersion)
        {
        }

        [Test]
        public void Should_ReturnGoodResult_When_RobotsTxtFound()
        {
            // Arrange
            mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = mockInstanceService.Object.CurrentInstance;

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));

            var baseUri = new Uri(mockInstance.Url);

            var expectedUri = new Uri(baseUri, Report.RobotsTxtRelative);

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnGoodResult_When_SiteIsInSubDirectoryAndRobotsTxtFound()
        {
            // Arrange

            mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.OK, out Mock<HttpMessageHandler> mockHttpMessageHandler);
            var mockInstance = mockInstanceService.Object.CurrentInstance;

            var baseUrl = mockInstance.Url;
            mockInstance.Url += "/subdirectory";

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));

            var expectedUri = new Uri($"{baseUrl}/{Report.RobotsTxtRelative}");

            AssertUrlCalled(mockHttpMessageHandler, expectedUri);
        }

        [Test]
        public void Should_ReturnWarningResult_When_RobotsTxtNotFound()
        {
            // Arrange
            mockReport = ArrangeReportAndHandlerWithHttpClientReturning(HttpStatusCode.NotFound, out _);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
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

            var report = ArrangeProperties(new Report(mockInstanceService.Object, httpClient));

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