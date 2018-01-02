using System;
using System.Runtime.Serialization;

namespace RESTful_Endpoints
{
    [DataContract]
    class DirectoryEntity
    {
        [DataMember]
        public int Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public int TypeId { get; set; }
        
        [DataMember]
        public string TypeDescription { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, TypeId: {2}, TypeDescription: {3}", Id, Name, TypeId, TypeDescription);
        }
    }
}