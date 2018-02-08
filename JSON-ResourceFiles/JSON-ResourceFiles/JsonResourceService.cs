using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JSON_ResourceFiles
{
    /*
     * Types deserialized should implement the DataContract attribute. Type members should implement the
     * DataMember attribute. For this example, we are deserializing a simple Dictionary<string, string>, 
     * which the DataContractJsonSerializer can deserialize out of the box. 
     */
    class JsonResourceService
    {
        public T GetResourceFromFile<T>(string jsonFilePath) 
            where T : class, new()
        {
            using (var fileStream = new FileStream(jsonFilePath, FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd())))
            {
                try
                {
                    var serializerSettings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                    var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>), serializerSettings);

                    return (T)(serializer.ReadObject(memoryStream));
                }
                catch (Exception innerException)
                {
                    var fileName = Path.GetFileName(jsonFilePath);
                    var message = string.Format("The {0} file is not a recognizable JSON dictionary format. Program will now exit.", fileName);
                    var exception = new ApplicationException(message, innerException);

                    throw exception;
                }
            }
        }
    }
}
