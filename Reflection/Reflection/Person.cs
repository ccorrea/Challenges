using System;
using System.Runtime.Serialization;

namespace Reflection
{
    [DataContract]
    class Person
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public Address Address { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public bool IsSuperHero { get; set; }
    }
}
