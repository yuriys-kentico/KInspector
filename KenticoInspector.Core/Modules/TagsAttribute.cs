using System;

using KenticoInspector.Core.Modules.Models;

namespace KenticoInspector.Core.Modules
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