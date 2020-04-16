using System;
using System.Collections.Generic;

using KenticoInspector.Core.TokenExpressions;
using KenticoInspector.Core.TokenExpressions.Models;

using NUnit.Framework;

namespace KenticoInspector.Core.Tests
{
    [TestFixture]
    public class SimpleTokenExpressionTests
    {
        public SimpleTokenExpressionTests()
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(Term).Assembly);
        }

        [TestCaseSource(
            typeof(TokenExpressionTestCases),
            nameof(TokenExpressionTestCases.ValidExpressionsWithOneSegment)
            )]
        [TestCaseSource(
            typeof(TokenExpressionTestCases),
            nameof(TokenExpressionTestCases.ValidExpressionsWithTwoSegments)
            )]
        [TestCaseSource(
            typeof(TokenExpressionTestCases),
            nameof(TokenExpressionTestCases.ValidExpressionsWithThreeSegments)
            )]
        [TestCaseSource(
            typeof(TokenExpressionTestCases),
            nameof(TokenExpressionTestCases.ValidExpressionsWithFourSegments)
            )]
        public string Should_ResolveExpression_When_ExpressionIsValid(
            string termString,
            object tokenValues
            )
        {
            // Arrange
            Term term = termString;
            var result = term.With(tokenValues);

            // Act
            return result.ToString();
        }

        [TestCaseSource(
            typeof(TokenExpressionTestCases),
            nameof(TokenExpressionTestCases.InvalidExpressionsWithOneSegment)
            )]
        public void Should_Throw_When_ExpressionIsInvalid(
            string termString,
            object tokenValues,
            Type exceptionType
            )
        {
            // Arrange
            Term term = termString;
            var result = term.With(tokenValues);

            // Act
            string resolvedTermMethod() => result.ToString();

            // Assert
            Assert.That(
                resolvedTermMethod,
                Throws.TypeOf(exceptionType)
                );
        }
    }

    public class TokenValues
    {
        public static object IsUndefined => new
        {
        };

        public static object IsNull => new
        {
            token = default(string)
        };

        public static object Is0 => new
        {
            token = 0
        };

        public static object Is1 => new
        {
            token = 1
        };

        public static object Is1_5 => new
        {
            token = 1.5
        };

        public static object Is5 => new
        {
            token = 5
        };

        public static object IsSome_Text => new
        {
            token = "some text"
        };

        public static object IsTrue => new
        {
            token = true
        };

        public static object IsFalse => new
        {
            token = false
        };

        public static object IsVersion => new
        {
            token = new Version("12.0.4")
        };
    }

    // TODO: Add valid expressions with leading space case
    // TODO: Add valid expressions with contains colon case
    // TODO: Add valid expressions with contains url case
    // TODO: Add valid expressions with multiple tokens

    public class TokenExpressionTestCases
    {
        private static string? category;
        private static string? tokenExpression;

        public static IEnumerable<TestCaseData> ValidExpressionsWithOneSegment()
        {
            category = "token only";
            tokenExpression = "<token>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "0"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "1"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "1.5"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "5"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "True"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "False"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "12.0.4"
                );
        }

        public static IEnumerable<TestCaseData> ValidExpressionsWithTwoSegments()
        {
            category = "token and default value";
            tokenExpression = "<token|resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "resolved"
                );

            category = "one integer case";
            tokenExpression = "<token|0:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one double case";
            tokenExpression = "<token|1.5:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one string case";
            tokenExpression = "<token|some text:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one true case";
            tokenExpression = "<token|true:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one version case";
            tokenExpression = "<token|12.0.4:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "resolved"
                );

            category = "one more than integer case";
            tokenExpression = "<token|>1:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one less than integer case";
            tokenExpression = "<token|<5:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );
        }

        public static IEnumerable<TestCaseData> ValidExpressionsWithThreeSegments()
        {
            category = "simple pluralization";
            tokenExpression = "<token|issue|issues>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "issues"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "issue"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "issues"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "issues"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "issues"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "issue"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "issues"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "issues"
                );

            category = "one integer case and default value";
            tokenExpression = "<token|1:resolved|not one>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not one"
                );

            category = "one double case and default value";
            tokenExpression = "<token|1.5:resolved|not one point five>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not one point five"
                );

            category = "one string case and default value";
            tokenExpression = "<token|some text:resolved|not some text>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not some text"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not some text"
                );

            category = "one true case and default value";
            tokenExpression = "<token|true:resolved|not true>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not true"
                );

            category = "one version case and default value";
            tokenExpression = "<token|12.0.4:resolved|not version 12>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not version 12"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "resolved"
                );

            category = "one more than integer case and default value";
            tokenExpression = "<token|>1:resolved|not more than>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not more than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not more than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not more than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not more than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not more than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not more than"
                );

            category = "one less than integer case and default value";
            tokenExpression = "<token|<5:resolved|not less than>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not less than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not less than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not less than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not less than"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not less than"
                );

            category = "two integer cases";
            tokenExpression = "<token|0:resolved|1:not zero>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not zero"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "two double cases";
            tokenExpression = "<token|9.9:not one point five|1.5:resolved>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "two string cases";
            tokenExpression = "<token|some text:resolved|other text:not some text>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one true case and one false case";
            tokenExpression = "<token|true:resolved|false:not true>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "two version cases";
            tokenExpression = "<token|12.0.4:resolved|8.1.18:not version 12>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "resolved"
                );

            category = "one more than integer case and one integer case";
            tokenExpression = "<token|>1:resolved|0:not more than 1>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not more than 1"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );

            category = "one less than integer case and one integer case";
            tokenExpression = "<token|<5:resolved|5:not less than 5>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not less than 5"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                ""
                );
        }

        public static IEnumerable<TestCaseData> ValidExpressionsWithFourSegments()
        {
            category = "two integer cases and default value";
            tokenExpression = "<token|1:resolved one|5:resolved five|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );

            category = "two double cases and default value";
            tokenExpression = "<token|1.5:resolved one point five|9.9:resolved nine point nine|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved one point five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );

            category = "two string cases and default value";
            tokenExpression = "<token|some text:resolved|other text: not some text|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "resolved"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );

            category = "one true case, one false case, and default value";
            tokenExpression = "<token|true:resolved true|false:resolved false|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "resolved true"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "resolved false"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );

            category = "two version cases and default value";
            tokenExpression = "<token|12.0.4:resolved|8.1.18:not version 12|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "resolved"
                );

            category = "one more than integer case, one integer case, and default value";
            tokenExpression = "<token|>1:resolved more than one|1:resolved one|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved more than one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved more than one"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );

            category = "one less than integer case, one integer case, and default value";
            tokenExpression = "<token|<5:resolved less than five|5:resolved five|not either>";

            yield return GetValidTestCaseWhen(
                TokenValues.IsUndefined,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsNull,
                ""
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is0,
                "resolved less than five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1,
                "resolved less than five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is1_5,
                "resolved less than five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.Is5,
                "resolved five"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsSome_Text,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsTrue,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsFalse,
                "not either"
                );

            yield return GetValidTestCaseWhen(
                TokenValues.IsVersion,
                "not either"
                );
        }

        public static IEnumerable<TestCaseData> InvalidExpressionsWithOneSegment()
        {
            category = "empty";
            tokenExpression = "<>";

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsUndefined,
                typeof(ArgumentException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsNull,
                typeof(ArgumentException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.Is1,
                typeof(ArgumentException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsSome_Text,
                typeof(ArgumentException)
                );

            category = "one segment and has a colon";
            tokenExpression = "<token:resolved>";

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsUndefined,
                typeof(FormatException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsNull,
                typeof(FormatException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.Is1,
                typeof(FormatException)
                );

            yield return GetInvalidTestCaseWhen(
                TokenValues.IsSome_Text,
                typeof(FormatException)
                );
        }

        private static TestCaseData GetValidTestCaseWhen(
            object tokenValues,
            string returns
            ) =>

            // TODO: add SetName($"\"{tokenExpression}\" with {tokenValues} returns \"{returns}\"") once NUnit fixes https://github.com/nunit/nunit3-vs-adapter/issues/607
            new TestCaseData(
                    tokenExpression,
                    tokenValues
                    )
                .Returns(returns)
                .SetCategory($"Token expression with {category}");

        private static TestCaseData GetInvalidTestCaseWhen(
            object tokenValues,
            Type throws
            ) =>

            //TODO: add SetName($"\"{tokenExpression}\" with {tokenValues} throws \"{throws.Name}\"") once NUnit fixes https://github.com/nunit/nunit3-vs-adapter/issues/607
            new TestCaseData(
                    tokenExpression,
                    tokenValues,
                    throws
                    )
                .SetCategory($"Token expression that is {category}");
    }
}