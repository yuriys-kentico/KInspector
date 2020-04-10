using System;
using System.Collections.Generic;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Instances.Repositories
{
    public interface IInstanceRepository : IRepository
    {
        Instance GetInstance(Guid guid);

        IList<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);

        void SaveInstances(IEnumerable<Instance> instances);

        bool DeleteInstance(Guid guid);
    }
}