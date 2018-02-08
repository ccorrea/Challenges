using System.Runtime.Serialization;

namespace Reflection
{
    [DataContract]
    class Address
    {
        [DataMember]
        public string AddressLine { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
