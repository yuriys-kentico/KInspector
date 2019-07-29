using System;
using System.Collections.Generic;
using System.Linq;
using KenticoInspector.Core.Constants;

namespace KenticoInspector.Core.Tokens
{
    /// <summary>
    /// Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<.*?>")]
    internal class SimpleTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary = new[] { '<', '>' };

        public string Resolve(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);

            var expression = TokenExpressionResolver.GetExpression
                <(string token, List<(string value, string caseValue)> expressionCases, string defaultValue)>
                (trimmedTokenExpression, TokenExpressionConstants.SimpleDelimiter, TokenExpressionConstants.CaseDelimiter);

            var resolved = false;

            var resolvedValue = string.Empty;

            if (tokenDictionary.TryGetValue(expression.token, out object token))
            {
                foreach (var (value, caseValue) in expression.expressionCases)
                {
                    var operation = TokenExpressionConstants.Equals;
                    resolved = TryResolveToken(token, value, operation, caseValue, out resolvedValue);

                    if (resolved) break;
                }

                if (resolved)
                {
                    return resolvedValue;
                }

                if (expression.token == expression.defaultValue)
                {
                    return token.ToString();
                }
            }
            else
            {
                return string.Empty;
            }

            return expression.defaultValue ?? string.Empty;
        }

        private bool TryResolveToken(object token, string value, char operation, string caseValue, out string resolvedValue)
        {
            switch (token)
            {
                case int intValue when token is int && intValue == 1:
                case int lessThanValue when token is int && operation == TokenExpressionConstants.LessThan && lessThanValue < int.Parse(value.ToString()):
                case int moreThanValue when token is int && operation == TokenExpressionConstants.MoreThan && moreThanValue > int.Parse(value.ToString()):
                case var _ when token?.ToString() == value:
                    resolvedValue = caseValue;

                    return true;
            }

            resolvedValue = null;

            return false;
        }
    }
}