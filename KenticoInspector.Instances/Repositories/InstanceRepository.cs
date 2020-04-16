using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Repositories;

using Newtonsoft.Json;

namespace KenticoInspector.Instances.Repositories
{
    public class InstanceRepository : IInstanceRepository
    {
        private readonly string SavedInstancesFilePath = $"{Directory.GetCurrentDirectory()}\\SavedInstances.json";

        public Instance GetInstance(Guid guid)
        {
            return GetInstances()
                .FirstOrDefault(instance => instance.Guid == guid);
        }

        public IList<Instance> GetInstances()
        {
            var saveFileExists = File.Exists(SavedInstancesFilePath);

            if (saveFileExists)
            {
                var saveFileContents = File.ReadAllText(SavedInstancesFilePath);
                var loadedInstances = JsonConvert.DeserializeObject<List<Instance>>(saveFileContents);

                return loadedInstances;
            }

            return new List<Instance>();
        }

        public Instance UpsertInstance(Instance instance)
        {
            instance.Guid = instance.Guid == Guid.Empty
                ? Guid.NewGuid()
                : instance.Guid;

            var savedInstances = GetInstances();

            if (savedInstances.All(savedInstance => savedInstance.Guid != instance.Guid))
            {
                savedInstances.Add(instance);
            }
            else
            {
                var existingInstanceIndex = savedInstances
                    .IndexOf(savedInstances.First(savedInstance => savedInstance.Guid == instance.Guid));

                savedInstances[existingInstanceIndex] = instance;
            }

            SaveInstances(savedInstances);

            return instance;
        }

        public void SaveInstances(IEnumerable<Instance> instances)
        {
            var jsonText = JsonConvert.SerializeObject(
                instances,
                Formatting.Indented
                );

            File.WriteAllText(
                SavedInstancesFilePath,
                jsonText
                );
        }

        public bool DeleteInstance(Guid guid)
        {
            var savedInstances = GetInstances();

            if (savedInstances.Any(savedInstance => savedInstance.Guid == guid))
            {
                SaveInstances(savedInstances.Where(savedInstances => savedInstances.Guid != guid));

                return true;
            }

            return false;
        }
    }
}