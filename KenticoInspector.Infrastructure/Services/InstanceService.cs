using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories;
using KenticoInspector.Core.Services;

namespace KenticoInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private Instance? currentInstance = null;

        private readonly IInstanceRepository _instanceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IDatabaseService _databaseService;

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
            _instanceRepository = instanceRepository;
            _versionRepository = versionRepository;
            _siteRepository = siteRepository;
            _databaseService = databaseService;
        }

        public bool DeleteInstance(Guid instanceGuid)
        {
            return _instanceRepository.DeleteInstance(instanceGuid);
        }

        public Instance GetInstance(Guid instanceGuid)
        {
            return _instanceRepository.GetInstance(instanceGuid);
        }

        public Instance SetCurrentInstance(Guid instanceGuid)
        {
            CurrentInstance = _instanceRepository.GetInstance(instanceGuid);

            return CurrentInstance;
        }

        public InstanceDetails GetInstanceDetails(Guid instanceGuid)
        {
            var instance = _instanceRepository.GetInstance(instanceGuid);

            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance? instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            _databaseService.Configure(instance.DatabaseSettings);

            return new InstanceDetails
            {
                Guid = instance.Guid,
                AdministrationVersion = _versionRepository.GetKenticoAdministrationVersion(instance),
                DatabaseVersion = _versionRepository.GetKenticoDatabaseVersion(instance),
                Sites = _siteRepository.GetSites(instance)
            };
        }

        public IList<Instance> GetInstances()
        {
            return _instanceRepository.GetInstances();
        }

        public Instance UpsertInstance(Instance instance)
        {
            return _instanceRepository.UpsertInstance(instance);
        }
    }
}