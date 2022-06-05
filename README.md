<h1 align="center">[SKYNET] INISerializer</h1>
<p align="center">This is a small library for Serialize - Deserialize objects and save them as ini settings files</p>
<p align="center">
    <img src="https://img.shields.io/nuget/dt/INISerializer?label=Nuget%20Downloads&style=for-the-badge" />
    <img src="https://img.shields.io/github/contributors/Hackerprod/-SKYNET-INISerializer?style=for-the-badge" />
    <img src="https://img.shields.io/github/forks/Hackerprod/-SKYNET-INISerializer?style=for-the-badge" alt="Project forks">
    <img src="https://img.shields.io/github/stars/Hackerprod/-SKYNET-INISerializer?label=Project%20Stars%21%21%21&style=for-the-badge" alt="Project stars">
    <img src="https://img.shields.io/github/issues/Hackerprod/-SKYNET-INISerializer?style=for-the-badge" alt="Project issues">
</p>


## Supported types
```
System.string
System.Net.IPAddress
System.Net.IPEndPoint
System.DateTime
System.TimeSpan
System.Enum
System.byte[]
System.Uri
System.Array
System.Version
System.Collections.Generic.List
System.Collections.Generic.Dictionary
System.Collections.Generic.KeyValuePair
System.Collections.Concurrent.ConcurrentDictionary
Primitive types(int, uint, long, decimal, bool, etc)
```

## How to use
Use the attribute [INISection("Section name")] as section name<br />
```csharp
[INISection("User Info")]
public string Name { get; set; }
```
```
[User Info]
Name = Hackerprod
```
Use the attribute [INIComment("Property description")] as property description<br />
```csharp
[INISection("User Info")]
[INIComment("Person name")]
public string Name { get; set; }
```
```
[User Info]
# Person name
Name = Hackerprod
```
Use the attribute [INISection("Section name", IsArray = true)] with IsArray for section array<br />
```csharp
[INISection("Phone book", IsArray = true)]
public Dictionary<string, long> Phonebook { get; set; }
```
```
[Phone book]
Jhon = 19544818009
Aria = 18564418075
```

## Code Example

### Class implementation

```csharp
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
```

### Serializer implementation

```csharp
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
```

Output
```
[User Info]
Name = Hackerprod
Age = 33
Language = spanish
Sex = Male
Height = 1,72
Birthday = 15/04/1989 12:00:00
Working = True
SecureCode = 76561198392279378
PersonalObject =
Welcome = 57656C636F6D6520746F20494E4953657269616C697A65722070726F6A656374

[Work Info]
# IP address of the user at work
IPAddress = 192.168.1.20
EndPoint = 192.168.1.20:80
Salary = 4000
Coworkers = Tania, Mercy, Kati
Jobs = [Employees, 8], [Director, 1], [Secretary, 2]
YearsWorking = 4369.12:46:30.9187699
URL = https://github.com/Hackerprod

[Phone book]
Daenerys = 17548081454
Jhon = 19544818009
Aria = 18564418075

[Phone book info]
BookVersion = 1.0.8
BookCategory = [Contacts, 3]
```

### Deserializer implementation

```csharp
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
```

Output
```
Name = Hackerprod
Age = 33
Language = spanish
Birthday = 15/04/1989 12:00:00
Sex = Male
Height = 1,72
Working = True
SecureCode = 76561198392279378
PersonalObject =
Welcome = Welcome to INISerializer project
IPAddress = 192.168.1.20
EndPoint = 192.168.1.20:80
Salary = 4000
Coworkers = Tania, Mercy, Kati
Jobs = [Employees, 8], [Director, 1], [Secretary, 2]
YearsWorking = 11,971321376166
Uri = https://github.com/Hackerprod
Phonebook = [Daenerys, 17548081454], [Jhon, 19544818009], [Aria, 18564418075]
BookVersion = 1.0.8
BookCategory = Contacts : 3
```

### Serialize and Deserialize to/from file

```csharp
string fileName = Path.Combine("c:/", "Settings.ini");

// Serialize and save to file
INISerializer.SerializeToFile(settings, fileName);

// Deserialize to object from file
Settings settings = INISerializer.DeserializeFromFile<Settings>(fileName);
```

### Debug Serialization and Deserialization errors

```csharp
INISerializer.OnErrorMessage += INISerializer_OnErrorMessage;
```
```csharp
private static void INISerializer_OnErrorMessage(object sender, string error)
{
    Console.WriteLine(error);
}
```

## Download from Nuget
https://www.nuget.org/packages/INISerializer/
