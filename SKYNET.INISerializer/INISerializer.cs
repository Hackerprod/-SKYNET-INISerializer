using System;
using System.IO;

namespace SKYNET.INI
{
    public class INISerializer : INIBase
    {
        /// <summary>
        /// Serialize an object with public fields and return serialization result
        /// </summary>
        /// <param name="obj">The instance of the object to serialize.</param>
        /// <returns>Content of serialized object</returns>
        public static string Serialize(object obj)
        {
            try
            {
                string Content = INISerialization.Serialize(obj);
                return Content;
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error serializing: {ex.ToString()}");
                return "";
            }
        }

        /// <summary>
        /// Serialize an object with public fields into an IniFile
        /// </summary>
        /// <param name="obj">The instance of the object to serialize.</param>
        /// <param name="filePath">Path to save the serialized object</param>
        public static void SerializeToFile(object obj, string filePath)
        {
            try
            {
                string content = Serialize(obj);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error serializing to file: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Deserialize the provided INI content into a type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize data into.</typeparam>
        /// <param name="content">Content into ini file</param>
        /// <returns></returns>
        public static T Deserialize<T>(string content) where T : class, new()
        {
            try
            {
                T obj = INIDeserialization.Deserialize<T>(content);
                return obj;
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error deserializing: {ex.ToString()}");
                return default;
            }
        }

        /// <summary>
        /// Deserialize the provided INI file into a type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize data into.</typeparam>
        /// <param name="content">Ini file path</param>
        /// <returns></returns>

        public static T DeserializeFromFile<T>(string fileName) where T : class, new()
        {
            try
            {
                string content = File.ReadAllText(fileName);
                T deserialized = (T)Deserialize<T>(content);
                return deserialized;
            }
            catch (Exception ex)
            {
                InvokeErrorMessage($"Error deserializing from file: {ex.ToString()}");
                return default;
            }
        }
    }
}
