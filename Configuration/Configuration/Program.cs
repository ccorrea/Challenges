using System;

namespace Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            var drinkFlavors = ConfigurationService.GetDrinkFlavors();

            Console.WriteLine("Available drink flavors are:");
            Console.WriteLine();

            foreach (var drinkFlavor in drinkFlavors)
            {
                if (drinkFlavor.IsSeasonal)
                {
                    Console.WriteLine($"{drinkFlavor.Name} ** Seasonal **");
                }
                else
                {
                    Console.WriteLine(drinkFlavor.Name);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
