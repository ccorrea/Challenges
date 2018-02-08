using System.Collections.Generic;

namespace Reflection
{
    class ComparisonResults
    {
        public List<ModelDifference> PropertiesOfDifferentValue { get; private set; }
        public List<string> PropertiesOfEqualValue { get; private set; }


        public ComparisonResults()
        {
            PropertiesOfDifferentValue = new List<ModelDifference>();
            PropertiesOfEqualValue = new List<string>();
        }

        public void AddDifferentResult(string propertyName, object valueOne, object valueTwo)
        {
            var modelDifference = new ModelDifference
            {
                PropertyName = propertyName,
                ValueOne = valueOne,
                ValueTwo = valueTwo
            };

            PropertiesOfDifferentValue.Add(modelDifference);
        }

        public void AddEqualResult(string propertyName)
        {
            PropertiesOfEqualValue.Add(propertyName);
        }
    }
}
