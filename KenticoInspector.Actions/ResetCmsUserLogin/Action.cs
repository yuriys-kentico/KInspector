using KenticoInspector.Actions.ResetCmsUserLogin.Models;
using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Modules;

using static KenticoInspector.Core.Modules.Models.Tags;

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
        protected override ActionResults GetResults(Options options) => new ActionResults
        {
            Status = ResultsStatus.NotRun,
            Summary = "Not implemented yet"
        };

        protected override ActionResults GetInvalidOptionsResults() => new ActionResults
        {
            Status = ResultsStatus.Error,
            Summary = Metadata.Terms.InvalidOptions
        };
    }
}