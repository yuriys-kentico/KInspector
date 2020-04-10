using System.Collections.Generic;

using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Modules.Repositories;

namespace KenticoInspector.Modules.Repositories
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