using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Reflection
{
    interface IModelService<T, H>
        where T : class
        where H : HashAlgorithm
    {
        string GetCryptographicHash(T anObject, H hashAlgorithm);
        List<ModelDifference> GetPropertiesOfDifferentValue(T objectOne, T objectTwo);
        List<string> GetPropertiesOfEqualValue(T objectOne, T objectTwo);
    }

    class ModelService<T, H> : IModelService<T, H>
        where T : class
        where H : HashAlgorithm
    {
        public string GetCryptographicHash(T anObject, H hashAlgorithm)
        {
            var type = anObject.GetType();
            var serializer = new DataContractSerializer(type);

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, anObject);
                hashAlgorithm.ComputeHash(memoryStream.ToArray());

                return Convert.ToBase64String(hashAlgorithm.Hash);
            }
        }
        
        public List<ModelDifference> GetPropertiesOfDifferentValue(T objectOne, T objectTwo)
        {
            var comparisonResults = CompareObjects(objectOne, objectTwo);

            return comparisonResults.PropertiesOfDifferentValue;
        }

        public List<string> GetPropertiesOfEqualValue(T objectOne, T objectTwo)
        {
            var comparisonResults = CompareObjects(objectOne, objectTwo);

            return comparisonResults.PropertiesOfEqualValue;
        }

        private ComparisonResults CompareObjects(T objectOne, T objectTwo)
        {
            var comparisonResults = new ComparisonResults();
            var objectType = objectOne.GetType();
            var properties = objectType.GetProperties();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var isValueType = propertyType.IsValueType;
                dynamic propertyValueOne = property.GetValue(objectOne);
                dynamic propertyValueTwo = property.GetValue(objectTwo);

                if (isValueType)
                {
                    if (propertyValueOne.Equals(propertyValueTwo))
                    {
                        comparisonResults.AddEqualResult(property.Name);
                    }
                    else
                    {
                        comparisonResults.AddDifferentResult(property.Name, propertyValueOne, propertyValueTwo);
                    }
                }
                else
                {
                    var bothAreNull = (propertyValueOne == null) && (propertyValueTwo == null);
                    var bothAreNotNull = (propertyValueOne != null) && (propertyValueTwo != null);
                    var onlyOneIsNull = (propertyValueOne == null) ^ (propertyValueTwo == null);

                    if (bothAreNull)
                    {
                        comparisonResults.AddEqualResult(property.Name);
                        continue;
                    }

                    if (bothAreNotNull)
                    {
                        var isString = propertyType.Equals(typeof(string));
                        var enumerableTypeName = typeof(IEnumerable<>).FullName;
                        var propertyInterface = propertyType.GetInterface(enumerableTypeName);
                        var isEnumerable = propertyInterface != null;
                        
                        if (isString)
                        {
                            if (propertyValueOne.Equals(propertyValueTwo))
                            {
                                comparisonResults.AddEqualResult(property.Name);
                            }
                            else
                            {
                                comparisonResults.AddDifferentResult(property.Name, propertyValueOne, propertyValueTwo);
                            }
                        }
                        else if (isEnumerable)
                        {
                            var areEqual = Enumerable.SequenceEqual(propertyValueOne, propertyValueTwo);

                            if (areEqual)
                            {
                                comparisonResults.PropertiesOfEqualValue.Add(property.Name);
                            }
                            else
                            {
                                comparisonResults.AddDifferentResult(property.Name, propertyValueOne, propertyValueTwo);
                            }
                        }
                        else
                        {
                            var hashAlgorithmType = typeof(HashAlgorithm);
                            var modelServiceType = typeof(ModelService<,>);
                            var genericType = modelServiceType.MakeGenericType(propertyType, hashAlgorithmType);
                            dynamic modelService = Activator.CreateInstance(genericType);

                            comparisonResults.PropertiesOfDifferentValue.AddRange(modelService.GetPropertiesOfDifferentValue(propertyValueOne, propertyValueTwo));
                            comparisonResults.PropertiesOfEqualValue.AddRange(modelService.GetPropertiesOfEqualValue(propertyValueOne, propertyValueTwo));
                        }

                        continue;
                    }

                    if (onlyOneIsNull)
                    {
                        comparisonResults.AddDifferentResult(property.Name, propertyValueOne, propertyValueTwo);
                        continue;
                    }
                }
            }

            return comparisonResults;
        }
    }
}
