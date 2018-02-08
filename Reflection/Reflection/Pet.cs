using System.Runtime.Serialization;

namespace Reflection
{
    [DataContract]
    class Pet
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PetType { get; set; }

        [DataMember]
        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Pet;

            if (other == null)
            {
                return false;
            }

            return 
                Name.Equals(other.Name) && 
                PetType.Equals(other.PetType) && 
                Age.Equals(other.Age);
        }
    }
}
