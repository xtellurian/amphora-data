using System;

namespace Amphora.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DatumMemberAttribute : Attribute
    {
        public DatumMemberAttribute(string name, string type, string units = null, string description = null)
        {
            Name = name;
            Type = type;
            Units = units;
            Description = description;
        }

        public string Name { get; }
        public string Type { get; }
        public string Units { get; }
        public string Description { get; }
    }
}