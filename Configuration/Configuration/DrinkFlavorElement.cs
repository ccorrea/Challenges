using System.Configuration;

namespace Configuration
{
    public class DrinkFlavorElement :  ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("seasonal", IsRequired = true)]
        public bool IsSeasonal
        {
            get
            {
                var value = this["seasonal"].ToString();

                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }

                return false;
            }

            set { this["seasonal"] = value; }
        }
    }
}
