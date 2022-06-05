﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.INI
{
    public class INIUtils
    {
        public static string ToHexString(byte[] data)
        {
            if (data == null)
                return null;
            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }
        public static byte[] FromHexString(string hexString)
        {
            if (hexString == null)
                return null;
            if (hexString.Length % 2 != 0)
                throw new FormatException("The hex string is invalid because it has an odd length");
            var result = new byte[hexString.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return result;
        }

        public static List<string> GetArrayElements(string value)
        {
            var Items = value.Split(new[] { "], [" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i] = Items[i].Replace("]", "").Replace("[", "");
            }
            return Items;
        }
    }

    public class Property
    {
        public string Section { get; set; }
        public bool IsArraySection { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public List<string> Comments { get; set; }
    }
}
