using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;

using Moq;

namespace KenticoInspector.Core.Tests.Mocks
{
    internal static class MockIDatabaseService
    {
        internal static Mock<IDatabaseService> Get()
        {
            var mock = new Mock<IDatabaseService>(MockBehavior.Strict);

            return mock;
        }

        internal static void SetupConfigure(
            this Mock<IDatabaseService> mockDatabaseService,
            Instance instance
            )
        {
            mockDatabaseService.Setup(mock => mock.Configure(instance.DatabaseSettings));
        }

        internal static void SetupExecuteSqlFromFile<T, U>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            string parameterPropertyName,
            IEnumerable<U> parameterPropertyValue,
            IEnumerable<T> returnValue
            )
        {
            mockDatabaseService
                .Setup(mock => mock.ExecuteSqlFromFile<T>(
                        script,
                        It.Is<object>(
                            objectToCheck => ObjectHasPropertyWithExpectedValue(objectToCheck, parameterPropertyName, parameterPropertyValue)
                        )
                    )
                )
                .Returns(returnValue);
        }

        internal static void SetupExecuteSqlFromFile<T>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            IDictionary<string, string> literalReplacements,
            string parameterPropertyName,
            IEnumerable<T> parameterPropertyValue,
            IEnumerable<IDictionary<string, object?>> returnValue)
        {
            mockDatabaseService
                .Setup(mock => mock.ExecuteSqlFromFile(
                        script,
                        literalReplacements,
                        It.Is<object>(
                            objectToCheck => ObjectHasPropertyWithExpectedValue(objectToCheck, parameterPropertyName, parameterPropertyValue)
                        )
                    )
                )
                .Returns(returnValue as IEnumerable<IDictionary<string, object>>);
        }

        internal static void SetupExecuteSqlFromFile<T>(
            this Mock<IDatabaseService> mockDatabaseService,
            string script,
            IEnumerable<T> returnValue)
        {
            mockDatabaseService
                .Setup(mock => mock.ExecuteSqlFromFile<T>(script))
                .Returns(returnValue);
        }

        private static bool ObjectHasPropertyWithExpectedValue<T>(object objectToCheck, string propertyName, IEnumerable<T> expectedValue)
        {
            var objectPropertyValue = objectToCheck
                .GetType()
                .GetProperty(propertyName)?
                .GetValue(objectToCheck, null) as IEnumerable<T>;

            return objectPropertyValue.SequenceEqual(expectedValue);
        }
    }
}