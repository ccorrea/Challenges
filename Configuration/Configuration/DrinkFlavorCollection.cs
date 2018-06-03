using System;
using System.Configuration;

namespace Configuration
{
    [ConfigurationCollection(typeof(DrinkFlavorElement))]
    public class DrinkFlavorCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Flavor";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMapAlternate;

        protected override string ElementName => PropertyName;

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DrinkFlavorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DrinkFlavorElement)(element)).Name;
        }

        public DrinkFlavorElement this[int i] => (DrinkFlavorElement)(BaseGet(i));
    }
}