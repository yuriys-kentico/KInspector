using KenticoInspector.Core.Instances.Services;

using Moq;

namespace KenticoInspector.Core.Tests.Mocks
{
    internal static class MockICmsFileService
    {
        internal static Mock<ICmsFileService> Get()
        {
            var mock = new Mock<ICmsFileService>(MockBehavior.Strict);

            return mock;
        }
    }
}