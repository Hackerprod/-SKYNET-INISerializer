using System;

namespace SKYNET.INI.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class INISectionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsArray { get; set; }

        public INISectionAttribute(string Name, bool IsArray = false) { this.Name = Name; this.IsArray = IsArray; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class INIComment : Attribute
    {
        public string Comment { get; set; }

        public INIComment(string Comment) { this.Comment = Comment; }
    }
}
