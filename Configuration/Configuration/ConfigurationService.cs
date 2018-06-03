using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class ConfigurationService
    {
        public static IEnumerable<DrinkFlavorElement> GetDrinkFlavors()
        {
            var configurationSection = (DrinkFlavorsSection)(ConfigurationManager.GetSection("drinkFlavors"));
            var drinkFlavors = configurationSection.DrinkFlavors;

            foreach (DrinkFlavorElement drinkFlavor in drinkFlavors)
            {
                if (drinkFlavor != null)
                {
                    yield return drinkFlavor;
                }
            }
        }
    }
}
