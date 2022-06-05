using SKYNET.INI.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class Settings
    {
        // User Info Section
        [INISection("User Info")]
        public string Name { get; set; }

        [INISection("User Info")]
        public int Age { get; set; }

        [INISection("User Info")]
        public string Language { get; set; }

        [INISection("User Info")]
        public SexType Sex { get; set; }

        [INISection("User Info")]
        public double Height { get; set; } 

        [INISection("User Info")]
        public DateTime Birthday { get; set; }

        [INISection("User Info")]
        public bool Working { get; set; }

        [INISection("User Info")]
        public long SecureCode { get; set; }

        [INISection("User Info")]
        public object PersonalObject { get; set; }

        [INISection("User Info")]
        public byte[] Welcome { get; set; }

        // Work Info Section
        [INISection("Work Info")]
        [INIComment("IP address of the user at work")]
        public IPAddress IPAddress { get; set; }

        [INISection("Work Info")]
        public IPEndPoint EndPoint { get; set; }

        [INISection("Work Info")]
        public uint Salary { get; set; }

        [INISection("Work Info")]
        public List<string> Coworkers { get; set; }

        [INISection("Work Info")]
        public ConcurrentDictionary<string, int> Jobs { get; set; }

        [INISection("Work Info")]
        public TimeSpan YearsWorking { get; set; }

        [INISection("Work Info")]
        public Uri URL { get; set; }

        // Phone book Section
        [INISection("Phone book", IsArray = true)]
        public Dictionary<string, long> Phonebook { get; set; }

        // Phone book info Section
        [INISection("Phone book info")]
        public Version BookVersion { get; set; }

        [INISection("Phone book info")]
        public KeyValuePair<string, int> BookCategory { get; set; }
    }

    public enum SexType
    {
        Unknown,
        Male,
        Female
    }
}
