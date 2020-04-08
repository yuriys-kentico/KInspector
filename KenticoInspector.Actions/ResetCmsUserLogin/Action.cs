using System.Collections.Generic;

using KenticoInspector.Actions.ResetCmsUserLogin.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;

namespace KenticoInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public override IList<string> Tags => new List<string> {
            ActionTags.Reset,
            ActionTags.Users
        };

        public Action(
            IDatabaseService databaseService,
            IInstanceService instanceService
            )
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        [SupportsVersions("10 - 12.0")]
        public override ActionResults GetResults(Options options)
        {
            return new ActionResults
            {
                Status = ResultsStatus.NotRun,
                Summary = "Not implemented yet"
            };
        }

        public override ActionResults GetInvalidOptionsResults()
        {
            return new ActionResults
            {
                Status = ResultsStatus.Error,
                Summary = Metadata.Terms.InvalidOptions
            };
        }
    }
}