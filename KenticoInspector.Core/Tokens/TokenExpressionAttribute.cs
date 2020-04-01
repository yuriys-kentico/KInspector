﻿using System;

namespace KenticoInspector.Core.Tokens
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TokenExpressionAttribute : Attribute
    {
        internal string Pattern { get; }

        public TokenExpressionAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}