using System.Configuration;

namespace Configuration
{
    public class DrinkFlavorsSection : ConfigurationSection
    {
        [ConfigurationProperty("Flavors")]
        public DrinkFlavorCollection DrinkFlavors
        {
            get { return (DrinkFlavorCollection)base["Flavors"]; }
            set { base["Flavors"] = value; }
        }
    }
}
