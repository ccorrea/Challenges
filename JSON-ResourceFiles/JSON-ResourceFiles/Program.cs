using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JSON_ResourceFiles
{
    class Program
    {
        static Random Random = new Random();
        static object Lock = new object();

        static void Main(string[] args)
        {
            var mascotsFilePath = GetMascotsFilePath();
            var noMascotsFileExists = !File.Exists(mascotsFilePath);

            if (noMascotsFileExists)
            {
                EndProgram("{0} mascots file not found. Program will now exit.", mascotsFilePath);
            }

            var mascotsDictionary = InitializeDictionary(mascotsFilePath);
            bool nameFound = false;
            var teamNames = mascotsDictionary.Keys.ToArray();

            while (!nameFound)
            {
                var randomTeamName = GetRandomElement(teamNames);

                Console.WriteLine("Please enter the name of a sports team (Example: {0}). Current mascots file is {1}.", randomTeamName, mascotsFilePath);

                var nameEntered = Console.ReadLine();

                nameFound = mascotsDictionary.ContainsKey(nameEntered);

                if (nameFound)
                {
                    var mascotOrMascots = mascotsDictionary[nameEntered];
                    var hasMultipleMascots = mascotOrMascots.Contains(",");
                    var prefix = hasMultipleMascots ? "Mascots are" : "Mascot is";

                    EndProgram("{0} {1}.", prefix, mascotOrMascots);
                }
                else
                {
                    Console.WriteLine("Sports team '{0}' is unknown. Please try again.", nameEntered);
                }
            }
        }

        private static void EndProgram(string message = null, params string[] arguments)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message, arguments);
            }

            Console.WriteLine("Press any key.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static string GetMascotsFilePath()
        {
            var settings = ConfigurationManager.AppSettings;

            if (settings.Count.Equals(0))
            {
                var executableName = AppDomain.CurrentDomain.FriendlyName;

                EndProgram("{0}.config file not found. Program will now exit.", executableName);
            }

            var mascotsFilePath = settings["MascotsFilePath"];
            var isRelativePath = Path.IsPathRooted(mascotsFilePath);

            if (isRelativePath)
            {
                mascotsFilePath = Path.Combine(Environment.CurrentDirectory, mascotsFilePath);
            }

            return mascotsFilePath;
        }

        private static string GetRandomElement(string[] array)
        {
            lock (Lock)
            {
                var lowerBound = array.GetLowerBound(0);
                var upperBound = array.GetUpperBound(0);
                var randomIndex = Random.Next(lowerBound, upperBound);
                var randomElement = array.ElementAt(randomIndex);

                return randomElement;
            }
        }

        private static Dictionary<string, string> InitializeDictionary(string jsonFilePath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var fileName = Path.GetFileName(jsonFilePath);

            using (var fileStream = new FileStream(jsonFilePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd())))
            {
                try
                {
                    var serializerSettings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                    var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>), serializerSettings);
                    var deserializedDictionary = (Dictionary<string, string>)(serializer.ReadObject(memoryStream));
                    
                    if (deserializedDictionary.Count.Equals(0))
                    {
                        EndProgram("The {0} file contains no mascot data. Program will now exit.", fileName);
                    }

                    dictionary = deserializedDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    EndProgram("The {0} file is not a recognizable JSON dictionary format. Program will now exit.", fileName);
                }
            }

            return dictionary;
        }
    }
}