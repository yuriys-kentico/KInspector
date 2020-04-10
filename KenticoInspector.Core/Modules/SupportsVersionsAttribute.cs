using System;

namespace KenticoInspector.Core.Modules
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SupportsVersionsAttribute : Attribute
    {
        public string CompatibleSemver { get; }

        public string IncompatibleSemver { get; }

        public SupportsVersionsAttribute(string supports = "*", string except = "")
        {
            CompatibleSemver = supports;
            IncompatibleSemver = except;
        }
    }
}