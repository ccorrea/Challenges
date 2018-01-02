using System;
using System.Runtime.Serialization;

namespace RESTful_Endpoints
{
    [DataContract]
    class SessionEntity
    {
        [DataMember]
        public string SessionCypher { get; set; }
        
        [DataMember]
        public DateTime ValidThrough { get; set; }

        public override string ToString()
        {
            return string.Format("Session Cypher: {0}..., Valid Through: {1}", SessionCypher.Substring(1, 10), ValidThrough.ToString("MM/dd/yyyy hh:mm"));
        }
    }
}