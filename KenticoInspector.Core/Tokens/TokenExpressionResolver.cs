using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Tokens
{
    public class TokenExpressionResolver
    {
        private const char space = ' ';

        private static IEnumerable<(Type tokenExpressionType, string pattern)> TokenExpressionTypePatterns { get; set; }

        private static string AllTokenExpressionPatterns => TokenExpressionTypePatterns
                                .Select(tokenExpressionTypePattern => tokenExpressionTypePattern.pattern)
                                .Where(ValueIsNotEmpty)
                                .Aggregate(AsRegexOr);

        public static void RegisterTokenExpressions(Assembly assemblies)
        {
            TokenExpressionTypePatterns = assemblies.GetTypes()
                                .Where(TypeIsMarkedWithTokenExpressionAttribute)
                                .Select(type => (type, GetTokenPatternFromAttribute(type)));
        }

        private static bool TypeIsMarkedWithTokenExpressionAttribute(Type type)
        {
            return type.IsDefined(typeof(TokenExpressionAttribute), false);
        }

        private static string GetTokenPatternFromAttribute(Type type)
        {
            var pattern = type
                .GetCustomAttributes<TokenExpressionAttribute>(false)
                .First()
                .Pattern;

            return $"( {pattern} )|({pattern} )|( {pattern}$)|(^{pattern}$)";
        }

        internal static string ResolveTokenExpressions(Term term, object tokenValues)
        {
            var tokenDictionary = GetValuesDictionary(tokenValues);

            return Regex.Split(term, AllTokenExpressionPatterns)
                        .Select(tokenExpression => ResolveTokenExpression(tokenExpression, tokenDictionary))
                        .Aggregate(AggregateStrings);
        }

        private static IDictionary<string, object> GetValuesDictionary(object tokenValues)
        {
            if (tokenValues is IDictionary<string, object> dictionary)
            {
                return dictionary;
            }

            return tokenValues
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(PropertyIsNotIndexableAndHasGetter)
                .ToDictionary(property => property.Name, property => property.GetValue(tokenValues));
        }

        private static bool PropertyIsNotIndexableAndHasGetter(PropertyInfo prop)
        {
            return prop.GetIndexParameters().Length == 0
                    && prop.GetMethod != null;
        }

        private static bool ValueIsNotEmpty(string pattern)
        {
            return !string.IsNullOrEmpty(pattern);
        }

        private static string AsRegexOr(string left, string right)
        {
            return $"{left}|{right}";
        }

        private static string ResolveTokenExpression(string tokenExpression, IDictionary<string, object> tokenDictionary)
        {
            var (leadingSpace, innerTokenExpression, trailingSpace) = GetSplitExpression<(char?, string, char?)>(tokenExpression);

            string resolvedExpression = null;

            foreach (var (tokenExpressionType, pattern) in TokenExpressionTypePatterns)
            {
                if (Regex.IsMatch(innerTokenExpression, pattern))
                {
                    var expressionObject = FormatterServices.GetUninitializedObject(tokenExpressionType) as ITokenExpression;

                    resolvedExpression = expressionObject.Resolve(innerTokenExpression, tokenDictionary);
                }
            }

            resolvedExpression = EnsureEmptyResolvedExpressionContainsOnlyOneSpace(ref leadingSpace, resolvedExpression, ref trailingSpace, innerTokenExpression);

            return $"{leadingSpace}{resolvedExpression}{trailingSpace}";
        }

        private static string EnsureEmptyResolvedExpressionContainsOnlyOneSpace(ref char? leadingSpace, string resolvedExpression, ref char? trailingSpace, string innerTokenExpression)
        {
            if (string.IsNullOrEmpty(resolvedExpression) && leadingSpace != null && trailingSpace != null)
            {
                leadingSpace = null;
                trailingSpace = null;

                return space.ToString();
            }
            else
            {
                return resolvedExpression ?? innerTokenExpression;
            }
        }

        private static T GetSplitExpression<T>(string tokenExpression)
        {
            char? leadingSpace = null;
            char? trailingSpace = null;

            if (tokenExpression.Any())
            {
                if (tokenExpression.First() == space) leadingSpace = space;
                if (tokenExpression.Last() == space) trailingSpace = space;
            }

            var props = typeof(T).GetProperties();

            return (leadingSpace, tokenExpression.Trim(), trailingSpace) as dynamic;
        }

        public static T GetExpression<T>(string tokenExpression, params char[] separators) where T : ITuple
        {
            var expressionProperties = typeof(T).GetGenericArguments();

            var expressionObject = GetExpressionObject(tokenExpression, expressionProperties, ref separators);

            return expressionObject as dynamic;
        }

        private static ITuple GetExpressionObject(string expression, Type[] expressionProperties, ref char[] separators)
        {
            if (separators.Length == 0 && expressionProperties.Length > 1)
            {
                throw new ArgumentException($"Provided null separators but expected to get {expressionProperties.Length} properties.");
            }

            if (separators.Length == 0 && expressionProperties.Length == 1)
            {
                return ValueTuple.Create(expression);
            }

            int currentSeparator = 0;

            var segments = expression.Split(separators[currentSeparator++]);

            var objectValues = GetObjectValuesFromSegments(segments, expressionProperties, ref separators, ref currentSeparator);

            return GetObject(objectValues, expressionProperties);
        }

        private static IList<dynamic> GetObjectValuesFromSegments(string[] segments, Type[] properties, ref char[] separators, ref int currentSeparator)
        {
            var objectValues = new List<dynamic>();

            int currentSegment = 0;

            int currentProperty = 0;

            foreach (var property in properties)
            {
                currentProperty++;

                bool getRemainingInReverse = false;

                var remainingSeparators = separators
                    .Skip(currentSeparator)
                    .ToArray();

                switch (property)
                {
                    case var _ when
                        property == typeof(string)
                        && currentSegment != segments.Length:

                        objectValues.Add(segments[currentSegment++]);
                        break;

                    case var iEnumerableProperty when
                        property != typeof(string)
                        && typeof(IEnumerable).IsAssignableFrom(property):

                        var iEnumerableGenericType = property.GetGenericArguments()[0];

                        var listType = typeof(List<>).MakeGenericType(iEnumerableGenericType);

                        var iEnumerableValues = Activator.CreateInstance(listType) as IList;

                        var segmentObjectProperties = iEnumerableProperty
                            .GetGenericArguments()[0]
                            .GetGenericArguments();

                        while (currentSegment < segments.Length)
                        {
                            var segment = segments[currentSegment++];

                            var segmentObject = GetExpressionObject(segment, segmentObjectProperties, ref remainingSeparators);

                            iEnumerableValues.Add(segmentObject);
                        }

                        objectValues.Add(iEnumerableValues);

                        getRemainingInReverse = true;

                        break;
                }

                if (getRemainingInReverse)
                {
                    var reversedProperties = properties
                        .Skip(currentProperty)
                        .Reverse()
                        .ToArray();

                    var reversedSegments = segments
                        .Reverse()
                        .Take(properties.Length - currentProperty)
                        .ToArray();

                    var remainingObjectValues = GetObjectValuesFromSegments(reversedSegments, reversedProperties, ref remainingSeparators, ref currentSeparator);

                    objectValues.AddRange(remainingObjectValues.Reverse());

                    break;
                }
            }

            return objectValues;
        }

        private static ITuple GetObject(IList<dynamic> objectValues, Type[] properties)
        {
            var expectedCount = properties.Length;

            switch (expectedCount)
            {
                case 1: return System.ValueTuple.Create(GetTypedValue(objectValues, properties, 0));
                case 2: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1));
                case 3: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2));
                case 4: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2), objectValues.ElementAtOrDefault(3));
                case 5: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2), objectValues.ElementAtOrDefault(3), objectValues.ElementAtOrDefault(4));
                case 6: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2), objectValues.ElementAtOrDefault(3), objectValues.ElementAtOrDefault(4), objectValues.ElementAtOrDefault(5));
                case 7: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2), objectValues.ElementAtOrDefault(3), objectValues.ElementAtOrDefault(4), objectValues.ElementAtOrDefault(5), objectValues.ElementAtOrDefault(6));
                case 8: return ValueTuple.Create(GetTypedValue(objectValues, properties, 0), GetTypedValue(objectValues, properties, 1), GetTypedValue(objectValues, properties, 2), objectValues.ElementAtOrDefault(3), objectValues.ElementAtOrDefault(4), objectValues.ElementAtOrDefault(5), objectValues.ElementAtOrDefault(6), objectValues.ElementAtOrDefault(7));
            }

            throw new NotSupportedException($"Creating a type with {expectedCount} values is not supported.");
        }

        private static dynamic GetTypedValue(IList<dynamic> objectValues, Type[] properties, int index)
        {
            var value = objectValues.ElementAtOrDefault(index);

            if (value == null)
            {
                var property = properties[index];

                switch (property)
                {
                    case var _ when property == typeof(string):
                        return string.Empty;
                }

                return Activator.CreateInstance(properties[index]);
            }

            return value;
        }

        private static string AggregateStrings(string left, string right)
        {
            return $"{left}{right}";
        }
    }
}