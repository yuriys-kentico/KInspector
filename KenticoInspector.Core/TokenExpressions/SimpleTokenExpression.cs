using System;
using System.Collections.Generic;
using System.Linq;

using static KenticoInspector.Core.TokenExpressions.Models.Constants;

namespace KenticoInspector.Core.TokenExpressions
{
    /// <summary>
    ///     Represents tokens using a single token and having N cases and an optional default.
    /// </summary>
    [TokenExpression("<(?!\\?).*?>")]
    internal class SimpleTokenExpression : ITokenExpression
    {
        private static readonly char[] expressionBoundary =
        {
            '<',
            '>'
        };

        public string? Resolve(
            string tokenExpression,
            IDictionary<string, object?> tokenDictionary
            )
        {
            var trimmedTokenExpression = tokenExpression.Trim(expressionBoundary);
            var expression = GetExpression(trimmedTokenExpression);

            if (tokenDictionary.TryGetValue(
                expression.token,
                out var token
                ))
                foreach (var (value, operation, result) in expression.cases)
                {
                    var resolved = TryResolveToken(
                        token,
                        value,
                        operation,
                        result,
                        out var resolvedToken
                        );

                    if (resolved) return resolvedToken;
                }

            if (token == null) return string.Empty;
            if (expression.token == expression.defaultValue) return token.ToString();

            return expression.defaultValue ?? string.Empty;
        }

        private (
            string token,
            IEnumerable<(string value, char operation, string result)> cases,
            string? defaultValue
            ) GetExpression(string tokenExpression)
        {
            if (string.IsNullOrEmpty(tokenExpression)) throw new ArgumentException($"'{tokenExpression}' looks like a simple token expression but does not contain a token.");

            var segments = tokenExpression.Split(Pipe);

            if (segments[0]
                .Contains(Colon)) throw new FormatException($"Simple token expression token '{segments[0]}' must not contain a {Colon}.");

            var cases = new List<(string, char, string)>();
            string? defaultValue = null;

            switch (segments.Length)
            {
                case 1:
                    defaultValue = segments[0];

                    break;

                default:
                    if (!segments[^1]
                        .Contains(Colon)) defaultValue = segments[^1];

                    cases.AddRange(
                        segments.Skip(1)
                            .Take(segments.Length - 1)
                            .Select(GetCase)
                        );

                    break;
            }

            return (segments[0], cases, defaultValue);
        }

        private static (string, char, string) GetCase(string expressionCase)
        {
            var operation = Equality;
            var first = expressionCase;
            var second = string.Empty;
            var index = expressionCase.IndexOf(Colon);

            if (index > -1)
            {
                var charSpan = expressionCase.AsSpan();
                first = new string(charSpan[..index]);
                second = new string(charSpan.Slice(index + 1));
            }

            if (!expressionCase.Contains(Colon)) return (string.Empty, operation, first);

            var firstChar = first[0];

            if (!OperationChars.Contains(firstChar)) return (first, operation, second);

            foreach (var item in OperationChars)
                if (firstChar == item)
                    operation = item;

            return (first.Substring(1), operation, second);
        }

        public bool TryResolveToken(
            object? token,
            string expressionCaseValue,
            char operation,
            string expressionCaseResult,
            out string resolvedValue
            )
        {
            var resolved = false;
            resolvedValue = string.Empty;

            if (token == null) return resolved;

            resolved = token switch
            {
                int intToken => TryResolveIntToken(
                    intToken,
                    expressionCaseValue,
                    operation
                    ),
                double doubleToken => TryResolveDoubleToken(
                    doubleToken,
                    expressionCaseValue,
                    operation
                    ),
                bool boolToken => TryResolveBoolToken(
                    boolToken,
                    expressionCaseValue
                    ),
                var _ => TryResolveStringToken(
                    token.ToString(),
                    expressionCaseValue
                    )
            };

            if (resolved) resolvedValue = expressionCaseResult;

            return resolved;
        }

        private static bool TryResolveIntToken(
            int token,
            string expressionCaseValue,
            char operation
            ) => TryResolveDoubleToken(
            token,
            expressionCaseValue,
            operation
            );

        private static bool TryResolveDoubleToken(
            double token,
            string expressionCaseValue,
            char operation
            )
        {
            var expressionCaseValueIsDouble = double.TryParse(
                expressionCaseValue,
                out var doubleExpressionCaseValue
                );

            if (expressionCaseValueIsDouble)
            {
                if (operation == Equality && token == doubleExpressionCaseValue
                    || operation == LessThan && token < doubleExpressionCaseValue
                    || operation == MoreThan && token > doubleExpressionCaseValue) return true;
            }
            else if (string.IsNullOrEmpty(expressionCaseValue) && token == 1)
            {
                return true;
            }

            return false;
        }

        private static bool TryResolveBoolToken(
            bool token,
            string expressionCaseValue
            )
        {
            var expressionCaseValueIsBool = bool.TryParse(
                expressionCaseValue,
                out var boolExpressionCaseValue
                );

            if (expressionCaseValueIsBool)
            {
                if (token == boolExpressionCaseValue) return true;
            }
            else if (string.IsNullOrEmpty(expressionCaseValue) && token)
            {
                return true;
            }

            return false;
        }

        private static bool TryResolveStringToken(
            string? token,
            string expressionCaseValue
            ) => token == expressionCaseValue;
    }
}