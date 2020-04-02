﻿using System;
using System.Collections.Generic;

using KenticoInspector.Actions.ResetCmsUserLogin.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms, Options>
    {
        private IDatabaseService databaseService;
        private IInstanceService instanceService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string> {
            ActionTags.Reset,
            ActionTags.Users
        };

        public Action(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IModuleMetadataService moduleMetadataService
            ) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override ActionResults Execute(Options Options)
        {
            return new ActionResults
            {
                Status = ResultsStatus.NotRun,
                Summary = "Not implemented yet"
            };
        }

        public override ActionResults GetInvalidOptionsResult()
        {
            return new ActionResults
            {
                Status = ResultsStatus.Error,
                Summary = Metadata.Terms.InvalidOptions
            };
        }
    }
}