using SKYNET.INI.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace SKYNET.INI
{
    internal class INIDeserialization : INIBase
    {
        private const string ArraySeparator = ", ";

        public static T Deserialize<T>(string content) where T : class, new()
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            List<Property> Properties = GetStringProperties(content);
            Type type = obj.GetType();
            foreach (var PropertyInfo in type.GetProperties())
            {
                if (PropertyInfo.IsDefined(typeof(INISectionAttribute)))
                {
                    var SectionAttribute = PropertyInfo.GetCustomAttribute<INISectionAttribute>();
                    object processedValue = null;
                    if (SectionAttribute.IsArray)
                    {
                        var properties = Properties.FindAll(p => p.Section == SectionAttribute.Name);
                        if (properties != null)
                        {
                            processedValue = ProcessArrayValue(properties, PropertyInfo.PropertyType);
                        }
                    }
                    else
                    {
                        var property = Properties.Find(p => p.Name == PropertyInfo.Name);
                        if (property != null)
                        {
                            processedValue = ProcessValue(property.Value.ToString(), PropertyInfo.PropertyType, PropertyInfo.Name);
                        }
                    }
                    try
                    {
                        MethodInfo? setMethod = PropertyInfo.GetSetMethod(true);
                        if (setMethod != null)
                            PropertyInfo.SetValue(obj, processedValue);
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage(ex.ToString());
                    }
                }
            }
            return obj;
        }

        private static List<Property> GetStringProperties(string content)
        {
            List<Property> Properties = new List<Property>();
            string CurrentSection = "";

            using (var reader = new StringReader(content))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (line.StartsWith("["))
                    {
                        CurrentSection = line.Replace("[", "").Replace("]", "");
                    }
                    else if (line.Contains("=") && !line.StartsWith("#"))
                    {
                        line = line.Replace(" =", "=").Replace("= ", "=");
                        string[] settingParts = line.Split('=');
                        if (settingParts.Length > 1)
                        {
                            var Key = settingParts[0];
                            var Value = "";
                            for (int s = 0; s < settingParts.Length; s++)
                            {
                                if (s != 0)
                                {
                                    Value += (s == settingParts.Length - 1) ? settingParts[s] : settingParts[s] + " ";
                                }
                            }
                            Property property = new Property()
                            {
                                Section = CurrentSection,
                                Name = Key,
                                Value = Value
                            };
                            Properties.Add(property);
                        }
                    }
                }
            }
            return Properties;
        }

        private static object ProcessValue(string value, Type PropertyType, string Name = "")
        {
            object processedValue = null;

            try
            {
                if (PropertyType == typeof(String))
                {
                    return value;
                }
                else if (PropertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value, out var Time))
                    {
                        return Time;
                    }
                }
                else if (PropertyType == typeof(IPAddress))
                {
                    if (IPAddress.TryParse(value, out var Address))
                    {
                        return Address;
                    }
                }
                else if (PropertyType.IsEnum)
                {
                    return Enum.Parse(PropertyType, value, false);
                }
                else if (PropertyType == typeof(object))
                {
                    return value;
                }
                else if (PropertyType == typeof(double))
                {
                    value = value.Replace(",", ".");
                    if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result))
                    {
                        return result;
                    }
                }
                else if (PropertyType == typeof(float))
                {
                    if (float.TryParse(value, out var result))
                    {
                        return result;
                    }
                }
                else if (PropertyType == typeof(decimal))
                {
                    if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                    {
                        return result;
                    }
                }
                else if (PropertyType.IsPrimitive)
                {
                    try
                    {
                        var result = Convert.ChangeType(value, PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                        return result;
                    }
                    catch
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (PropertyType == typeof(byte[]))
                {
                    var result = INIUtils.FromHexString(value.ToString());
                    return result;
                }
                else if (value != null && PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    try
                    {
                        Type listType = PropertyType.GetGenericArguments()[0];
                        List<string> elems = value.Split(new string[] { ArraySeparator }, StringSplitOptions.None).ToList();
                        var list = (IList)PropertyType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count });
                        for (int i = 0; i < elems.Count; i++)
                        {
                            list.Add(ProcessValue(elems[i], listType));
                        }
                        return list;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (value != null && PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)))
                {
                    try
                    {
                        Type KeyType = PropertyType.GetGenericArguments()[0];
                        Type ValueType = PropertyType.GetGenericArguments()[1];

                        List<string> Items = INIUtils.GetArrayElements(value);
                        var dictionary = (IDictionary)PropertyType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { Items.Count / 2 });
                        foreach (string item in Items)
                        {
                            List<string> KV = item.Split(new string[] { ArraySeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            var Key = ProcessValue(KV[0], KeyType);
                            var Value = ProcessValue(KV[1], ValueType);
                            dictionary.Add(Key, Value);
                        }
                        return dictionary;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ConcurrentDictionary<,>)))
                {
                    try
                    {
                        Type KeyType = PropertyType.GetGenericArguments()[0];
                        Type ValueType = PropertyType.GetGenericArguments()[1];

                        var Constructor = PropertyType.GetConstructor(new Type[] { });
                        dynamic dictionary = Constructor.Invoke(new object[] { });

                        MethodInfo SetMethod = PropertyType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, ValueType, new[] { KeyType }, null)?.GetSetMethod();
                        if (SetMethod == null) return null;

                        List<string> Items = INIUtils.GetArrayElements(value);
                        foreach (string item in Items)
                        {
                            try
                            {
                                List<string> KV = item.Split(new string[] { ArraySeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                var Key = ProcessValue(KV[0], KeyType);
                                var Value = ProcessValue(KV[1], ValueType);
                                SetMethod.Invoke(dictionary, new object[] { Key, Value });
                            }
                            catch (Exception ex)
                            {
                                InvokeErrorMessage(ex.ToString());
                            }
                        }
                        return dictionary;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (value != null && PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(KeyValuePair<,>)))
                {
                    try
                    {
                        Type KeyType = PropertyType.GetGenericArguments()[0];
                        Type ValueType = PropertyType.GetGenericArguments()[1];

                        var Constructor = PropertyType.GetConstructor(new[] { KeyType, ValueType });
                        List<string> Items = INIUtils.GetArrayElements(value);
                        foreach (string item in Items)
                        {
                            List<string> KV = item.Split(new string[] { ArraySeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            var Key = ProcessValue(KV[0], KeyType);
                            var Value = ProcessValue(KV[1], ValueType);
                            dynamic keyvalue = Constructor.Invoke(new object[] { Key, Value });
                            return keyvalue;
                        }
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (value != null && PropertyType.IsArray)
                {
                    try
                    {
                        Type arrayType = PropertyType.GetElementType();
                        List<string> elems = value.Split(new string[] { ArraySeparator }, StringSplitOptions.None).ToList();
                        Array newArray = Array.CreateInstance(arrayType, elems.Count);
                        for (int i = 0; i < elems.Count; i++)
                        {
                            newArray.SetValue(ProcessValue(elems[i], arrayType), i);
                        }
                        return newArray;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (PropertyType == typeof(TimeSpan))
                {
                    if (TimeSpan.TryParse(value.ToString(), out var span))
                    {
                        return span;
                    }
                }
                else if (PropertyType == typeof(Uri))
                {
                    if (Uri.TryCreate(value.ToString(), UriKind.RelativeOrAbsolute, out var uri))
                    {
                        return uri;
                    }
                }
                else if (PropertyType == typeof(System.Version))
                {
                    if (System.Version.TryParse(value.ToString(), out var version))
                    {
                        return version;
                    }
                }
                else if (PropertyType == typeof(IPEndPoint))
                {
                    try
                    {
                        if (value.ToString().Contains(":"))
                        {
                            var Parts = value.ToString().Split(':');
                            var IpAddress = IPAddress.Parse(Parts[0]);
                            var Port = int.Parse(Parts[1]);
                            IPEndPoint EndPoint = new IPEndPoint(IpAddress, Port);
                            return EndPoint;
                        }
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else if (PropertyType == typeof(EndPoint))
                {
                    try
                    {
                        if (value.ToString().Contains(":"))
                        {
                            var Parts = value.ToString().Split(':');
                            var IpAddress = IPAddress.Parse(Parts[0]);
                            var Port = int.Parse(Parts[1]);
                            IPEndPoint EndPoint = new IPEndPoint(IpAddress, Port);
                            return (EndPoint)EndPoint;
                        }
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
                    }
                }
                else
                {
                    InvokeErrorMessage($"Not found type for {Name} {PropertyType}");
                }
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error Deserializing {Name}, Type {PropertyType}");
            }
            return processedValue;
        }

        private static object ProcessArrayValue(List<Property> Properties, Type PropertyType)
        {
            try
            {
                if (PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)))
                {
                    var dictionary = (IDictionary)PropertyType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { Properties.Count / 2 });
                    try
                    {
                        Type KeyType = PropertyType.GetGenericArguments()[0];
                        Type ValueType = PropertyType.GetGenericArguments()[1];

                        foreach (var Property in Properties)
                        {
                            string Key = Property.Name;
                            string Value = Property.Value.ToString();
                            dictionary.Add(ProcessValue(Key, KeyType), ProcessValue(Value, ValueType));
                        }
                        return dictionary;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage(ex.ToString());
                    }
                }
                else if (PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ConcurrentDictionary<,>)))
                {
                    Type KeyType = PropertyType.GetGenericArguments()[0];
                    Type ValueType = PropertyType.GetGenericArguments()[1];

                    var Constructor = PropertyType.GetConstructor(new Type[] { });
                    dynamic dictionary = Constructor.Invoke(new object[] { });

                    MethodInfo SetMethod = PropertyType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, ValueType, new[] { KeyType }, null)?.GetSetMethod();

                    if (SetMethod == null) return null;

                    try
                    {
                        foreach (var Property in Properties)
                        {
                            try
                            {
                                var Key = ProcessValue(Property.Name, KeyType);
                                var Value = ProcessValue(Property.Value.ToString(), ValueType);
                                SetMethod.Invoke(dictionary, new object[] { Key, Value });
                            }
                            catch (Exception ex)
                            {
                                InvokeErrorMessage(ex.ToString());
                            }
                        }
                        return dictionary;
                    }
                    catch (Exception ex)
                    {
                        InvokeErrorMessage(ex.ToString());
                    }
                }
                else
                {
                    InvokeErrorMessage("Section array is only valid in IDictionary types");
                    return null;
                }
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error processing array section value: {ex.ToString()}");
            }
            return null;
        }
    }
}
