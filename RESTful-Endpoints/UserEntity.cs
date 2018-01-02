using System;
using System.Runtime.Serialization;

namespace RESTful_Endpoints
{
    [DataContract]
    class UserEntity
    {
        [DataMember]
        public string EmailAddress { get; set; }
        
        [DataMember]
        public string Password { get; set; }

        public override string ToString()
        {
            return string.Format("E-Mail: {0}, Password: {1}", EmailAddress, Password);
        }
    }
}