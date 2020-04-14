using System;
using System.Collections.Generic;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Instances.Services
{
    public interface IInstanceService : IService
    {
        Instance CurrentInstance { get; }

        bool DeleteInstance(Guid instanceGuid);

        Instance GetInstance(Guid instanceGuid);

        InstanceDetails GetInstanceDetails(Guid instanceGuid);

        InstanceDetails GetInstanceDetails(Instance? instance = null);

        IList<Instance> GetInstances();

        Instance SetCurrentInstance(Guid instanceGuid);

        Instance UpsertInstance(Instance instance);
    }
}