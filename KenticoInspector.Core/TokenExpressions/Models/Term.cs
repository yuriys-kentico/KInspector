﻿namespace KenticoInspector.Core.TokenExpressions.Models
{
    public class Term
    {
        private string Markdown { get; }

        private object? TokenValues { get; set; }

        private Term(string value)
        {
            Markdown = value;
        }

        public static implicit operator Term(string value) => new Term(value);

        public static implicit operator string(Term term) => term.ToString();

        public override string ToString()
        {
            if (TokenValues != null)
                return TokenExpressionResolver.ResolveTokenExpressions(
                    Markdown,
                    TokenValues
                    );

            return Markdown;
        }

        /// <summary>
        ///     Prepares for token replacement based on the <paramref name="tokenValues" /> object.
        /// </summary>
        /// <param name="tokenValues">
        ///     Object with property names that map to token names and property values that map to token
        ///     values.
        /// </param>
        public Term With(object tokenValues)
        {
            TokenValues = tokenValues;

            return this;
        }
    }
}