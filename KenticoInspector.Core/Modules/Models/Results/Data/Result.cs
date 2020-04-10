using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Core.Modules.Models.Results.Data
{
    public abstract class Result
    {
        public string? Label { get; protected set; }

        public abstract bool HasData { get; }

        public static implicit operator Result(Term term) => new StringResult(term);
    }
}