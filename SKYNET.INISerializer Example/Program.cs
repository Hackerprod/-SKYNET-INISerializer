using SKYNET.INI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SKYNET
{
    class Program
    {
        static void Main(string[] args)
        {
            INISerializer.OnErrorMessage += INISerializer_OnErrorMessage;

            Settings created = new Settings()
            {
                Name = "Hackerprod",
                Age = 33,
                Language = "spanish",
                Birthday = new DateTime(1989, 04, 15),
                Sex = SexType.Male,
                Height = 1.72,
                Working = true,
                SecureCode = 76561198392279378,
                PersonalObject = null,
                Welcome = Encoding.Default.GetBytes("Welcome to INISerializer project"),
                IPAddress = IPAddress.Parse("192.168.1.20"),
                EndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.20"), 80),
                Salary = 4000,
                Coworkers = new List<string>() { "Tania", "Mercy", "Kati" },
                Jobs = new ConcurrentDictionary<string, int>(new Dictionary<string, int>() { { "Secretary", 2 }, { "Director", 1 }, { "Employees", 8 } }),
                YearsWorking = DateTime.Now - (new DateTime(2010, 06, 18)),
                URL = new Uri("https://github.com/Hackerprod"),
                Phonebook = new Dictionary<string, long>() { { "Daenerys", 17548081454 }, { "Jhon", 19544818009 }, { "Aria", 18564418075 } },
                BookVersion = new Version("1.0.8"),
                BookCategory = new KeyValuePair<string, int>("Contacts", 3)
            };

            string serializedSettings = INISerializer.Serialize(created);

            Console.WriteLine(serializedSettings);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press Any key to Deserialize object");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.ReadKey();

            Settings deserialized = INISerializer.Deserialize<Settings>(serializedSettings);

            Console.WriteLine(
            "Name = "            + deserialized.Name + Environment.NewLine +
            "Age = "             + deserialized.Age + Environment.NewLine +
            "Language = "        + deserialized.Language + Environment.NewLine +
            "Birthday = "        + deserialized.Birthday + Environment.NewLine +
            "Sex = "             + deserialized.Sex + Environment.NewLine +
            "Height = "          + deserialized.Height + Environment.NewLine +
            "Working = "         + deserialized.Working + Environment.NewLine +
            "SecureCode = "      + deserialized.SecureCode + Environment.NewLine +
            "PersonalObject = "  + deserialized.PersonalObject + Environment.NewLine +
            "Welcome = "         + Encoding.Default.GetString(deserialized.Welcome) + Environment.NewLine +
            "IPAddress = "       + deserialized.IPAddress + Environment.NewLine +
            "EndPoint = "        + deserialized.EndPoint + Environment.NewLine +
            "Salary = "          + deserialized.Salary + Environment.NewLine +
            "Coworkers = "       + String.Join(", ", deserialized.Coworkers) + Environment.NewLine +
            "Jobs = "            + String.Join(", ", deserialized.Jobs) + Environment.NewLine +
            "YearsWorking = "    + deserialized.YearsWorking.TotalDays / 365 + Environment.NewLine +
            "Uri = "             + deserialized.URL + Environment.NewLine +
            "Phonebook = "       + String.Join(", ", deserialized.Phonebook) + Environment.NewLine +
            "BookVersion = "     + deserialized.BookVersion + Environment.NewLine +
            "BookCategory = "    + deserialized.BookCategory.Key + " : " + deserialized.BookCategory.Value
            );

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Press Any key to exit");
            Console.ReadKey();
        }

        private static void INISerializer_OnErrorMessage(object sender, string error)
        {
            Console.WriteLine(error);
        }
    }
}
