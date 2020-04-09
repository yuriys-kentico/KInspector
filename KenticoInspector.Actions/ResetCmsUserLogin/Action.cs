using KenticoInspector.Actions.ResetCmsUserLogin.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Infrastructure;

using static KenticoInspector.Core.Models.Tags;

namespace KenticoInspector.Actions.ResetCmsUserLogin
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Action(
            IDatabaseService databaseService,
            IInstanceService instanceService
            )
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        [Tags(Reset, Users)]
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