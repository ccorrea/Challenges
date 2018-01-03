using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JSON_Deserialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            
            client.BaseAddress = new Uri("http://files.olo.com");

            var task = client.GetAsync("pizzas.json");
            var responseMessage = task.Result;
            var pizzas = GetEntity<IEnumerable<Pizza>>(responseMessage);
            var dictionary = new Dictionary<string, int>();

            foreach (var pizza in pizzas)
            {
                var key = pizza.ToString();
                
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, 1);
                    continue;        
                }

                dictionary[key]++;
            }

            var keyValuePairs = dictionary.ToList();

            keyValuePairs.Sort((pairOne, pairTwo) => pairOne.Value.CompareTo(pairTwo.Value));
            keyValuePairs.Reverse();
            
            var topKeyValuePairs = keyValuePairs.GetRange(0, 20);

            for (var index = 0; index < topKeyValuePairs.Count; index++)
            {
                var keyValuePair = topKeyValuePairs[index];
                var rank = index + 1;

                Console.WriteLine("#{0}: {1} ({2} orders)", rank, keyValuePair.Key, keyValuePair.Value);
            }
        }
       
        private static T GetEntity<T>(HttpResponseMessage responseMessage)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var content = responseMessage.Content;
            var task = content.ReadAsStreamAsync();
            var stream = task.Result;           
            var entity = (T)(serializer.ReadObject(stream));
            
            return entity;
        }

        [DataContract]
        class Pizza
        {
            [DataMember(Name = "toppings")]
            public HashSet<string> Toppings { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                
                var otherPizza = (Pizza)obj;

                return otherPizza.Toppings.Equals(Toppings);
            }
            
            public override int GetHashCode()
            {
                return Toppings.GetHashCode();
            }

            public override string ToString()
            {
                const string prefix = "Pizza with ";
                var suffix = "no toppings";

                if(Toppings != null)
                {
                    var toppings = new string[Toppings.Count];
                    
                    Toppings.CopyTo(toppings, 0);
                    Array.Sort(toppings);

                    suffix = string.Join(", ", toppings);
                }
                
                return string.Concat(prefix, suffix);
            }
        }
    }
}
