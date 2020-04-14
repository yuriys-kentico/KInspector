using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Modules.Services;
using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Core.TokenExpressions;
using KenticoInspector.Core.TokenExpressions.Models;
using KenticoInspector.Modules.Models;
using KenticoInspector.Modules.Services;

using NUnit.Framework;

namespace KenticoInspector.Infrastructure.Tests
{
    public class TestTerms
    {
        public Term SingleTerm { get; set; } = null!;
    }

    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ModuleMetadataServiceTests
    {
        private readonly IModuleMetadataService moduleMedatadataService;

        public ModuleMetadataServiceTests(int majorVersion)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(Term).Assembly);

            var mockInstance = MockInstances.Get(majorVersion);
            var mockInstanceDetails = MockInstanceDetails.Get(majorVersion, mockInstance);

            var mockInstanceService = MockIInstanceService.Get();
            mockInstanceService.SetupCurrentInstance(mockInstance, mockInstanceDetails);

            moduleMedatadataService = new ModuleMetadataService(mockInstanceService.Object);
        }

        [TestCaseSource(typeof(YamlTestCases), nameof(YamlTestCases.YamlMatchesModel))]
        public void Should_Resolve_When_YamlMatchesModel(
            string cultureName,
            string yamlPath,
            ModuleMetadata<TestTerms> resolvedMetadata)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            // Act
            var metadata = moduleMedatadataService.GetModuleMetadata(yamlPath, typeof(TestTerms), Array.Empty<Tags>());

            // Assert
            Assert.That(metadata.Details.Name, Is.EqualTo(resolvedMetadata.Details.Name));
            Assert.That(metadata.Details.ShortDescription, Is.EqualTo(resolvedMetadata.Details.ShortDescription));
            Assert.That(metadata.Details.LongDescription, Is.EqualTo(resolvedMetadata.Details.LongDescription));

            Assert.That((metadata as dynamic).Terms.SingleTerm.ToString(), Is.EqualTo(resolvedMetadata.Terms.SingleTerm.ToString()));
        }

        [TestCase("en-US", "TestData\\YamlDoesNotMatch", TestName = "Metadata throws exception when YAML does not match the model.")]
        public void Should_Throw_When_YamlDoesNotMatchModel(
            string cultureName,
            string yamlPath)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            // Act
            IModuleMetadata getModuleMetadata(string path) => moduleMedatadataService
                    .GetModuleMetadata(path, typeof(TestTerms), Array.Empty<Tags>());

            // Assert
            Assert.That(() => getModuleMetadata(yamlPath), Throws.Exception);
        }
    }

    public class YamlTestCases
    {
        public static IEnumerable<TestCaseData> YamlMatchesModel
        {
            get
            {
                yield return GetTestCaseData("en-US", "TestData\\YamlMatches", new ModuleMetadata<TestTerms>
                {
                    Details = new ModuleDetails
                    {
                        Name = "Details name",
                        ShortDescription = "Details shortDescription",
                        LongDescription = "Details longDescription\n"
                    },
                    Terms = new TestTerms
                    {
                        SingleTerm = "Term value"
                    }
                });
                yield return GetTestCaseData("en-GB", "TestData\\YamlMatches", new ModuleMetadata<TestTerms>
                {
                    Details = new ModuleDetails
                    {
                        Name = "British details name",
                        ShortDescription = "Details shortDescription",
                        LongDescription = "Details longDescription\n"
                    },
                    Terms = new TestTerms
                    {
                        SingleTerm = "British term value"
                    }
                });
            }
        }

        private static TestCaseData GetTestCaseData(
            string cultureCode,
            string yamlPath,
            ModuleMetadata<TestTerms> resolvedMetadata)
        {
            return new TestCaseData(cultureCode, yamlPath, resolvedMetadata);
            //TODO: add .SetName($"Metadata in culture \"{cultureCode}\" resolves."); once NUnit fixes https://github.com/nunit/nunit3-vs-adapter/issues/607
        }
    }
}