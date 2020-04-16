using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;

using Moq;

namespace KenticoInspector.Core.Tests.Mocks
{
    internal static class MockIInstanceService
    {
        internal static Mock<IInstanceService> Get()
        {
            var mock = new Mock<IInstanceService>(MockBehavior.Strict);

            return mock;
        }

        internal static void SetupCurrentInstance(
            this Mock<IInstanceService> mockInstanceService,
            Instance instance,
            InstanceDetails instanceDetails
            )
        {
            mockInstanceService.Setup(mock => mock.CurrentInstance)
                .Returns(instance);

            mockInstanceService.Setup(mock => mock.GetInstance(instance.Guid))
                .Returns(instance);

            mockInstanceService.Setup(mock => mock.GetInstanceDetails(instance))
                .Returns(instanceDetails);

            mockInstanceService.Setup(mock => mock.GetInstanceDetails(null))
                .Returns(instanceDetails);
        }
    }
}