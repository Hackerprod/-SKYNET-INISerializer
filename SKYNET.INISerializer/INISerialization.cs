using SKYNET.INI.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SKYNET.INI
{
    internal class INISerialization : INIBase
    {
        internal static string Serialize(object obj)
        {
            var Properties = new List<Property>();

            Type type = obj.GetType();
            foreach (var PropertyInfo in type.GetProperties())
            {
                if (PropertyInfo.IsDefined(typeof(INISectionAttribute)))
                {
                    var Property = new Property();
                    var Comments = new List<string>();

                    var SectionAttribute = PropertyInfo.GetCustomAttribute<INISectionAttribute>();

                    if (PropertyInfo.IsDefined(typeof(INIComment)))
                    {
                        var CommentAttribute = PropertyInfo.GetCustomAttributes<INIComment>();
                        foreach (var attribute in CommentAttribute)
                        {
                            Comments.Add(attribute.Comment);
                        }
                    }

                    Property.Section = SectionAttribute.Name;
                    Property.IsArraySection = SectionAttribute.IsArray;
                    Property.Name = PropertyInfo.Name;
                    Property.Value = PropertyInfo.GetValue(obj, null);
                    Property.Comments = Comments;

                    Properties.Add(Property);
                }
            }

            StringBuilder builder = new StringBuilder();
            string currentSection = "";

            foreach (var Property in Properties)
            {
                if (Property.Section != currentSection)
                {
                    if (!string.IsNullOrEmpty(currentSection)) builder.AppendLine();
                    builder.AppendLine($"[{Property.Section}]");
                    currentSection = Property.Section;
                }

                if (Property.Comments.Any())
                {
                    foreach (var comment in Property.Comments)
                    {
                        builder.AppendLine($"# {comment}");
                    }
                }

                object parsedValue = Property.Value;
                if (parsedValue != null)
                {
                    parsedValue = ParseValue(parsedValue, Property.IsArraySection);
                }
                if (Property.IsArraySection)
                {
                    if (parsedValue != null && parsedValue.ToString().Length != 0)
                    {
                        builder.AppendLine($"{parsedValue}");
                    }
                }
                else
                {
                    builder.AppendLine($"{Property.Name} = {parsedValue}");
                }
            }

            return builder.ToString();
        }

        private static object ParseValue(object value, bool IsArray = false)
        {
            var type = value.GetType();
            object parsedValue = value;

            try
            {
                if (type == typeof(System.Byte[]))
                {
                    parsedValue = INIUtils.ToHexString((byte[])value);
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    dynamic items = parsedValue;
                    parsedValue = String.Join(", ", items);
                }
                else if (type.IsGenericType && (type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) || type.GetGenericTypeDefinition().IsAssignableFrom(typeof(ConcurrentDictionary<,>))))
                {
                    dynamic items = parsedValue;
                    parsedValue = "";
                    if (IsArray)
                    {
                        int i = 0;
                        foreach (var item in items)
                        {
                            parsedValue += $"{ParseValue(item.Key)} = {ParseValue(item.Value)}";
                            if (i != items.Count - 1) parsedValue += Environment.NewLine;
                            i++;
                        }
                    }
                    else
                    {
                        parsedValue = String.Join(", ", items);
                    }
                }
                else if (type.IsArray)
                {
                    dynamic items = parsedValue;
                    parsedValue = "";
                    for (int i = 0; i < items.Length; i++)
                    {
                        parsedValue += "\"" + ParseValue(items[i]) + "\"";
                        if (i != items.Length - 1) parsedValue += ", ";
                    }
                }
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"{ex.Message}: {ex.StackTrace}");
            }

            return parsedValue;
        }

    }
}
