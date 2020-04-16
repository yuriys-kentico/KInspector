using System;

namespace KenticoInspector.Core.TokenExpressions
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TokenExpressionAttribute : Attribute
    {
        internal string Pattern { get; }

        public TokenExpressionAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}