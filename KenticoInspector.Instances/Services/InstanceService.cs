using System;
using System.Collections.Generic;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Repositories;
using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Instances.Services
{
    public class InstanceService : IInstanceService
    {
        private Instance? currentInstance = null;

        private readonly IInstanceRepository instanceRepository;
        private readonly ISiteRepository siteRepository;
        private readonly IVersionRepository versionRepository;
        private readonly IDatabaseService databaseService;

        public Instance CurrentInstance
        {
            get => currentInstance ?? throw new InvalidOperationException();
            private set => currentInstance = value;
        }

        public InstanceService(
            IInstanceRepository instanceRepository,
            IVersionRepository versionRepository,
            ISiteRepository siteRepository,
            IDatabaseService databaseService)
        {
            this.instanceRepository = instanceRepository;
            this.versionRepository = versionRepository;
            this.siteRepository = siteRepository;
            this.databaseService = databaseService;
        }

        public bool DeleteInstance(Guid instanceGuid)
        {
            return instanceRepository.DeleteInstance(instanceGuid);
        }

        public Instance GetInstance(Guid instanceGuid)
        {
            return instanceRepository.GetInstance(instanceGuid);
        }

        public Instance SetCurrentInstance(Guid instanceGuid)
        {
            CurrentInstance = instanceRepository.GetInstance(instanceGuid);

            return CurrentInstance;
        }

        public InstanceDetails GetInstanceDetails(Guid instanceGuid)
        {
            var instance = instanceRepository.GetInstance(instanceGuid);

            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance? instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            databaseService.Configure(instance.DatabaseSettings);

            return new InstanceDetails
            {
                Guid = instance.Guid,
                AdministrationVersion = versionRepository.GetKenticoAdministrationVersion(instance),
                DatabaseVersion = versionRepository.GetKenticoDatabaseVersion(instance),
                Sites = siteRepository.GetSites(instance)
            };
        }

        public IList<Instance> GetInstances()
        {
            return instanceRepository.GetInstances();
        }

        public Instance UpsertInstance(Instance instance)
        {
            return instanceRepository.UpsertInstance(instance);
        }
    }
}