using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RESTful_Endpoints
{
    class Program
    {
        static SessionEntity SessionEntity { get; set; }
        static UserEntity UserEntity { get; set; }

        static void Main(string[] args)
        {
            UserEntity = PostUserEntity();
            SessionEntity = PostSessionEntity(UserEntity);

            Console.WriteLine(UserEntity);
            Console.WriteLine(SessionEntity);
            WriteDirectoryEntities();
        }

        private static HttpClient CreateHttpClient(bool useSession = true)
        {
            var client = new HttpClient();
            var uri = new Uri("https://my.rpsins.com/resume/api/");

            client.BaseAddress = uri;

            if (useSession && SessionEntity != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionEntity.SessionCypher);
            }

            return client;
        }
        
        private static T GetEntity<T>(HttpResponseMessage responseMessage)
        {
            var settings = new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss.fffffffK") };
            var serializer = new DataContractJsonSerializer(typeof(T), settings);
            var content = responseMessage.Content;
            var task = content.ReadAsStreamAsync();
            var stream = task.Result;
            var entity = (T)(serializer.ReadObject(stream));
            
            return entity;
        }

        private static StringContent GetStringContent<T>(T entity)
        {
            var serializer = new DataContractJsonSerializer(entity.GetType());
            var memoryStream = new MemoryStream();
            
            serializer.WriteObject(memoryStream, entity);
            
            var json = GetStringFromStream(memoryStream);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return stringContent;
        }

        private static string GetStringFromStream(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            
            stream.Position = 0;
            
            return streamReader.ReadToEnd();
        }

        private static SessionEntity PostSessionEntity(UserEntity userEntity)
        {
            var client = CreateHttpClient(false);
            var content = GetStringContent<UserEntity>(userEntity);
            var task = client.PostAsync("Session", content);
            var responseMessage = task.Result;
            var sessionEntity = GetEntity<SessionEntity>(responseMessage);

            return sessionEntity;
        }

        private static UserEntity PostUserEntity()
        {
            var client = CreateHttpClient(false);
            var userEntity = new UserEntity { EmailAddress = "user@server.com", Password = "Test" };
            var content = GetStringContent<UserEntity>(userEntity);
            var responseMessage = client.PostAsync("User", content);

            return userEntity;
        }

        private static void WriteDirectoryEntities(DirectoryEntity entity = null)
        {
            var requestUri = "DirectoryEntries";
            
            if (entity != null)
            {
                requestUri += string.Concat("/", entity.Id);
            }

            var client = CreateHttpClient();
            var task = client.GetAsync(requestUri);
            var responseMessage = task.Result;
            var entities = GetEntity<IEnumerable<DirectoryEntity>>(responseMessage);

            foreach (var fileOrFolder in entities)
            {
                if (fileOrFolder.TypeDescription.Equals("Folder"))
                {
                    WriteDirectoryEntities(fileOrFolder);
                }
                else
                {
                    WriteFileContent(fileOrFolder);
                }
            }
        }

        private static void WriteFileContent(DirectoryEntity entity)
        {
            var client = CreateHttpClient();
            var uri = string.Format("DirectoryEntries/{0}/Content", entity.Id);
            var task = client.GetStringAsync(uri);

            Console.WriteLine(entity);
            Console.WriteLine(task.Result);
            Console.WriteLine();
        }

    }
}
