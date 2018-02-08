using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Reflection
{
    class Program
    {
        static public Person[] People
        {
            get
            {
                /*
                 * Person[0] and Person[1] have the same last name and address, everything else is different.
                 * Person[2] and Person[3] have the same date of birth and middle name, everything else is different.
                 */
                
                return new Person[]
                {
                    new Person
                    {
                        FirstName = "Bartholomew",
                        MiddleName = "Henry",
                        LastName = "Allen",
                        Address = Places[0],
                        DateOfBirth = DateTime.Parse("1995-04-01"),
                        IsSuperHero = true,
                    },
                    new Person
                    {
                        FirstName = "Iris",
                        LastName = "Allen",
                        Address = Places[0],
                        DateOfBirth = DateTime.Parse("1995-02-01"),
                        IsSuperHero = false
                    },
                    new Person
                    {
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        Address = Places[1],
                        DateOfBirth = DateTime.Parse("1975-12-01"),
                        IsSuperHero = true,
                    },
                    new Person
                    {
                        FirstName = "John",
                        LastName = "Diggle",
                        Address = Places[2],
                        DateOfBirth = DateTime.Parse("1975-12-01"),
                        IsSuperHero = false
                    },
                };
            }
        }

        static public Address[] Places
        {
            get
            {
                return new Address[]
                {
                    new Address
                    {
                        AddressLine = "123 Fast Lane",
                        City = "Central City",
                        State = "IL",
                    },
                    new Address
                    {
                        AddressLine = "1 Wayne Lane",
                        City = "Gotham City",
                        State = "NY",
                    },
                    new Address
                    {
                        AddressLine = "45 Spartan Drive",
                        City = "Star City",
                        State = "WA",
                    },
                };
            }
        }

        static void Main(string[] args)
        {
            var shaServiceProvider = new SHA1CryptoServiceProvider();
            var shaModelService = new ModelService<Person, SHA1>();
            
            Console.WriteLine("*** EXAMPLE 1: Persons 0 and 1 have the same last name and address, everything else is different ***");
            
            WriteComparisonResults(shaModelService, People[0], People[1]);

            Console.WriteLine("*** EXAMPLE 2: Persons 2 and 3 have the same middle name and birth date, everything else is different ***");

            WriteComparisonResults(shaModelService, People[2], People[3]);

            Console.WriteLine("*** EXAMPLE 3: SHA1 cryptographic hashes for all objects are:");

            WriteCryptographicHashes(person => shaModelService.GetCryptographicHash(person, shaServiceProvider));

            Console.WriteLine("*** EXAMPLE 4: MD5 cryptographic hashes for all objects are:");

            var md5ServiceProvider = new MD5CryptoServiceProvider();
            var md5ModelService = new ModelService<Person, MD5>();

            WriteCryptographicHashes(person => md5ModelService.GetCryptographicHash(person, md5ServiceProvider));

            Console.WriteLine("Press any key.");
            Console.ReadKey();
        }

        private static object ValueOrNull(object value)
        {
            return value ?? "NULL";
        }

        private static void WriteComparisonResults(ModelService<Person, SHA1> modelService, Person personOne, Person personTwo)
        {
            var equalPropertyNames = modelService.GetPropertiesOfEqualValue(personOne, personTwo);
            var modelDifferences = modelService.GetPropertiesOfDifferentValue(personOne, personTwo);
            
            Console.WriteLine("Properties with equal value: {0}", string.Join(", ", equalPropertyNames));

            foreach (var difference in modelDifferences)
            {
                Console.WriteLine("{0} property is different: {1} != {2}.", difference.PropertyName, ValueOrNull(difference.ValueOne), ValueOrNull(difference.ValueTwo));
            }

            Console.WriteLine();
        }

        private static void WriteCryptographicHashes(Func<Person, string> hashFunction)
        {
            foreach (var person in People)
            {
                Console.WriteLine(hashFunction(person));
            }

            Console.WriteLine();
        }
    }
}
