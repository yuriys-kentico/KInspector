using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using static KenticoInspector.Core.TokenExpressions.Models.Constants;

namespace KenticoInspector.Core.TokenExpressions
{
    public static class TokenExpressionResolver
    {
        private static IEnumerable<(Type tokenExpressionType, string pattern)>? TokenExpressionTypePatterns { get; set; }

        public static void RegisterTokenExpressions(Assembly assembly)
        {
            TokenExpressionTypePatterns = assembly
                .GetTypes()
                .Where(TypeIsMarkedWithTokenExpressionAttribute)
                .Select(AsTokenExpressionTypePattern);

            static bool TypeIsMarkedWithTokenExpressionAttribute(Type type)
            {
                return type.IsDefined(typeof(TokenExpressionAttribute), false);
            }

            static (Type type, string) AsTokenExpressionTypePattern(Type type)
            {
                var pattern = type
                    .GetCustomAttributes<TokenExpressionAttribute>(false)
                    .First()
                    .Pattern;

                var patternVariants = new[]
                {
                    $"([{new string(LeadingChars)}]{pattern}[{new string(TrailingChars)}])",
                    $"({pattern}[{new string(TrailingChars)}])",
                    $"([{new string(LeadingChars)}]{pattern})",
                    $"(^{pattern}$)"
                };

                var joinedPattern = string.Join(Pipe, patternVariants);

                return (type, joinedPattern);
            }
        }

        internal static string ResolveTokenExpressions(string term, object tokenValues)
        {
            var allTokenExpressionPatterns = TokenExpressionTypePatterns
                .Select(tokenExpressionTypePattern => tokenExpressionTypePattern.pattern)
                .Where(pattern => !string.IsNullOrEmpty(pattern));

            var joinedTokenExpressionPatterns = string.Join(Pipe, allTokenExpressionPatterns);

            var tokenDictionary = GetValuesDictionary(tokenValues);

            var resolvedExpressions = Regex.Split(term, joinedTokenExpressionPatterns)
                .Select(tokenExpression => ResolveTokenExpression(tokenExpression, tokenDictionary));

            return string.Join(string.Empty, resolvedExpressions);
        }

        private static IDictionary<string, object?> GetValuesDictionary(object tokenValues)
        {
            if (tokenValues is IDictionary<string, object?> dictionary)
            {
                return dictionary;
            }

            return tokenValues
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetIndexParameters().Length == 0 && prop.GetMethod != null)
                .ToDictionary(property => property.Name, property => property.GetValue(tokenValues));
        }

        private static string ResolveTokenExpression(string tokenExpression, IDictionary<string, object?> tokenDictionary)
        {
            var (leadingChar, innerTokenExpression, trailingChar) = GetSplitExpression(tokenExpression);

            string? resolvedExpression = null;

            if (TokenExpressionTypePatterns == null)
            {
                throw new Exception($"'{nameof(TokenExpressionTypePatterns)}' should be set using '{nameof(RegisterTokenExpressions)}'.");
            }

            foreach (var (tokenExpressionType, pattern) in TokenExpressionTypePatterns)
            {
                if (Regex.IsMatch(innerTokenExpression, pattern))
                {
                    var expressionObject = FormatterServices.GetUninitializedObject(tokenExpressionType) as ITokenExpression;

                    resolvedExpression = expressionObject?.Resolve(innerTokenExpression, tokenDictionary);

                    break;
                }
            }

            return $"{leadingChar}{resolvedExpression ?? innerTokenExpression}{trailingChar}";
        }

        private static (char?, string, char?) GetSplitExpression(string tokenExpression)
        {
            switch (tokenExpression.Length)
            {
                case 0:
                case 1:
                    return (null, tokenExpression, null);

                case 2:
                default:
                    var tokenExpressionSpan = tokenExpression.AsSpan();

                    char? leadingChar = tokenExpressionSpan[0];
                    char? trailingChar = tokenExpressionSpan[tokenExpression.Length - 1];

                    if (!LeadingChars.Contains(leadingChar.Value))
                    {
                        leadingChar = null;
                    }
                    else
                    {
                        tokenExpression = tokenExpression.Substring(1);
                    }

                    if (!TrailingChars.Contains(trailingChar.Value))
                    {
                        trailingChar = null;
                    }
                    else
                    {
                        tokenExpression = tokenExpression[0..^1];
                    }

                    return (leadingChar, tokenExpression, trailingChar);
            }
        }
    }
}