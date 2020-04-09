using System;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core
{
    public sealed class TagsAttribute : Attribute
    {
        public Tags[] Tags { get; }

        public TagsAttribute(params Tags[] tags)
        {
            Tags = tags;
        }
    }
}