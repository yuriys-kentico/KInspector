using System.Collections.Generic;

using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Repositories
{
    public interface IActionRepository : IRepository
    {
        IEnumerable<IAction> GetActions();
    }
}