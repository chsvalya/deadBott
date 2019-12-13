using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot
{
    class Testcs
    {
        public static void Some()
        {
            Dictionary<int, string> countries = new Dictionary<int, string>();
            countries.Add(1, "Russia");
            countries.Add(3, "Great Britain");
            countries.Add(2, "USA");
            countries.Add(4, "France");
            countries.Add(5, "China");
            var c = countries.Keys;
            Console.WriteLine(countries.Keys.Where(x => x == 1));
        }
    }
}
