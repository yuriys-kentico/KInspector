using System.Collections.Generic;

namespace KenticoInspector.Core.TokenExpressions
{
    internal interface ITokenExpression
    {
        string? Resolve(string tokenExpression, IDictionary<string, object?> tokenDictionary);
    }
}