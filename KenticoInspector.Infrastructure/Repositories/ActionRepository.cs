using System.Collections.Generic;

using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Repositories;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly IEnumerable<IAction> actions;

        public ActionRepository(IEnumerable<IAction> actions)
        {
            this.actions = actions;
        }

        public IEnumerable<IAction> GetActions() => actions;
    }
}