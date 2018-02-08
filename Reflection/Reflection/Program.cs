using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Reflection
{
    class Program
    {
        #region Sample People

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
                        Pets = new[] { Pets[0], Pets[1] },
                    },
                    new Person
                    {
                        FirstName = "Iris",
                        LastName = "Allen",
                        Address = Places[0],
                        DateOfBirth = DateTime.Parse("1995-02-01"),
                        IsSuperHero = false,
                        Pets = new[] { Pets[0], Pets[1] },
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
                        IsSuperHero = false,
                        Pets = new[] { Pets[2] },
                    },
                };
            }
        }

        #endregion

        #region Sample Places

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

        #endregion

        #region Sample Pets

        static public Pet[] Pets
        {
            get
            {
                return new Pet[]
                {
                    new Pet { Name = "Bolt", Age = 2, PetType = "Dog" },
                    new Pet { Name = "Dash", Age = 3, PetType = "Cat" },
                    new Pet { Name = "Fido", Age = 4, PetType = "Dog" },
                };
            }
        }

        #endregion
        
        static void Main(string[] args)
        {
            var cryptoProviderOne = new SHA1CryptoServiceProvider();
            var modelServiceOne = new ModelService<Person, SHA1>();
            
            WriteComparisonResults(modelServiceOne, People, 0, 1);
            WriteComparisonResults(modelServiceOne, People, 2, 3);

            var modelServiceTwo = new ModelService<Person, MD5>();
            var cryptoProviderTwo = new MD5CryptoServiceProvider();

            WriteCryptographicHashes(People, modelServiceOne, cryptoProviderOne);
            WriteCryptographicHashes(People, modelServiceTwo, cryptoProviderTwo);

            Console.WriteLine("Press any key.");
            Console.ReadKey();
        }

        private static object ValueOrNull(object value)
        {
            return value ?? "NULL";
        }

        private static void WriteComparisonResults(ModelService<Person, SHA1> modelService, Person[] people, int indexOne, int indexTwo)
        {
            var personOne = people[indexOne];
            var personTwo = people[indexTwo];
            var equalPropertyNames = modelService.GetPropertiesOfEqualValue(personOne, personTwo);
            var modelDifferences = modelService.GetPropertiesOfDifferentValue(personOne, personTwo);
            var equalPropertiesListed = string.Join(", ", equalPropertyNames);

            Console.WriteLine("*** Persons {0} and {1} have the same {2}. ***", indexOne, indexTwo, equalPropertiesListed);

            foreach (var difference in modelDifferences)
            {
                Console.WriteLine("{0} property is different: {1} != {2}.", difference.PropertyName, ValueOrNull(difference.ValueOne), ValueOrNull(difference.ValueTwo));
            }

            Console.WriteLine();
        }

        private static void WriteCryptographicHashes<H>(IEnumerable<Person> people, ModelService<Person, H> modelService, H hashAlgorithm)
            where H : HashAlgorithm
        {
            var algorithmType = hashAlgorithm.GetType();
            var algorithmName = algorithmType.Name;

            Console.WriteLine("*** Cryptographic hashes by {0} ***", algorithmName);

            foreach (var person in people)
            {
                Console.WriteLine(modelService.GetCryptographicHash(person, hashAlgorithm));
            }

            Console.WriteLine();
        }
    }
}
