﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Repositories;
using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Instances.Repositories
{
    public class VersionRepository : IVersionRepository
    {
        private const string getCmsSettingsPath = @"Scripts/GetCmsSettings.sql";

        private const string administrationDllToCheck = "CMS.DataEngine.dll";
        private const string relativeAdministrationDllPath = "bin";
        private const string relativeHotfixFileFolderPath = "App_Data\\Install";
        private const string hotfixFile = "Hotfix.txt";
        private readonly IDatabaseService databaseService;

        public VersionRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public Version GetKenticoAdministrationVersion(Instance instance) => GetKenticoAdministrationVersion(instance.Path);

        public Version GetKenticoAdministrationVersion(string rootPath)
        {
            if (!Directory.Exists(rootPath)) throw new DirectoryNotFoundException();

            var binDirectory = Path.Combine(
                rootPath,
                relativeAdministrationDllPath
                );

            if (!Directory.Exists(binDirectory)) throw new DirectoryNotFoundException();

            var dllFileToCheck = Path.Combine(
                binDirectory,
                administrationDllToCheck
                );

            if (!File.Exists(dllFileToCheck)) throw new DirectoryNotFoundException();

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(dllFileToCheck);
            var hotfix = "0";

            var hotfixDirectory = Path.Combine(
                rootPath,
                relativeHotfixFileFolderPath
                );

            if (Directory.Exists(hotfixDirectory))
            {
                var hotfixFile = Path.Combine(
                    hotfixDirectory,
                    VersionRepository.hotfixFile
                    );

                if (File.Exists(hotfixFile)) hotfix = File.ReadAllText(hotfixFile);
            }

            var version = $"{fileVersionInfo.FileMajorPart}.{fileVersionInfo.FileMinorPart}.{hotfix}";

            return new Version(version);
        }

        public Version GetKenticoDatabaseVersion(Instance instance) => GetKenticoDatabaseVersion(instance.DatabaseSettings);

        public Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings)
        {
            var settingsKeys = databaseService.ExecuteSqlFromFile<string>(getCmsSettingsPath)
                .ToList();

            var version = settingsKeys[0];
            var hotfix = settingsKeys[1];

            return new Version($"{version}.{hotfix}");
        }
    }
}