using System;
using System.Globalization;
using System.Collections;

namespace Collation
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            string string1 = "hello gus";
            string string2 = "HELLO GUS";
            int compareResult = 0;

            // Like Java String.compareTo (without culture,
            // using unicode values directly)
            compareResult = String.Compare(string1, string2,
                StringComparison.Ordinal);
            Console.WriteLine("{0} comparison of '{1}' and '{2}': {3}", 
                StringComparison.Ordinal, string1, string2,
                compareResult);

            // Like Java String.compareToIgnoreCase (without culture,
            // using unicode values directly)
            compareResult = String.Compare(string1, string2,
                StringComparison.OrdinalIgnoreCase);
            Console.WriteLine("{0} comparison of '{1}' and '{2}': {3}", 
                StringComparison.OrdinalIgnoreCase, string1, string2, 
                compareResult);
                
            // Like Java String.toLowerCase(Locale.ENGLISH) 
            string key = "HELLO GUS";
            string lowerKey = key.ToLower (CultureInfo.InvariantCulture);
            Console.WriteLine ("Key: {0}", lowerKey);

            string[] words = {"cote", "coté", "côte", "côté"};

            // C# unlike Java does not implement Levels/Strengths collation configurations.
            // It always uses the five levels, see: Mono.Globalization.Unicode/SimpleCollator.cs

            // InvariantCulture: English  like Java: Locale.ENGLISH
            Array.Sort (words, StringComparer.InvariantCulture);
            Console.WriteLine("Words list Invariant: ");
            printValues (words);

            // without culture, using unicode values directly
            Array.Sort (words, StringComparer.Ordinal);
            Console.WriteLine("Words list Ordinal: ");
            printValues (words);

            // like Java new Locale("es","ES")
            CultureInfo cultureES = CultureInfo.CreateSpecificCulture ("es-ES");
            IComparer cultureComparerES =new CaseInsensitiveComparer (cultureES);
            Array.Sort (words, cultureComparerES);
            Console.WriteLine("Words list ES: ");
            printValues (words);

            CultureInfo cultureFR = CultureInfo.CreateSpecificCulture ("fr-FR");
            IComparer cultureComparerFR =new CaseInsensitiveComparer (cultureFR);
            Array.Sort (words, cultureComparerFR);
            Console.WriteLine("Words list FR: ");
            printValues (words);

        }

        private static void printValues(string[] words)
        {
            foreach (var word in words)
            {
                Console.Write ("{0} ", word);
            }

            Console.WriteLine ();
        }
    }
}
