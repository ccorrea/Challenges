using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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

            var mascotsDictionary = new Dictionary<string, string>();

            try
            {
                mascotsDictionary = InitializeDictionary(mascotsFilePath);
            }
            catch (Exception exception)
            {
                EndProgram(exception.Message);
            }

            if (mascotsDictionary.Count.Equals(0))
            {
                EndProgram("{0} mascots file contains no data. Program will now exit.");
            }

            var mascotsFileName = Path.GetFileName(mascotsFilePath);
            bool nameFound = false;
            var teamNames = mascotsDictionary.Keys.ToArray();

            while (!nameFound)
            {
                var randomTeamName = GetRandomElement(teamNames);
                
                Console.WriteLine("Please enter the name of a sports team (Example: {0}). Current mascots file is {1}.", randomTeamName, mascotsFileName);

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
            var isRelativePath = !Path.IsPathRooted(mascotsFilePath);

            if (isRelativePath)
            {
                mascotsFilePath = Path.Combine(Environment.CurrentDirectory, mascotsFilePath);
            }

            return mascotsFilePath;
        }

        private static string GetRandomElement(string[] array)
        {
            var lowerBound = array.GetLowerBound(0);
            var upperBound = array.GetUpperBound(0);
            int randomIndex;

            lock (Lock)
            {
                randomIndex = Random.Next(lowerBound, upperBound);
            }

            return array.ElementAt(randomIndex);
        }

        private static Dictionary<string, string> InitializeDictionary(string jsonFilePath)
        {
            var jsonResouceService = new JsonResourceService();
            var dictionary = jsonResouceService.GetResourceFromFile<Dictionary<string, string>>(jsonFilePath);

            if (dictionary.Count.Equals(0))
            {
                var fileName = Path.GetFileName(jsonFilePath);
                var message = string.Format("The {0} file contains no mascot data.", fileName);

                throw new ApplicationException(message);
            }
            
            return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}