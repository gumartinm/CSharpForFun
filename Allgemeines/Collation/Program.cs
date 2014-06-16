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

            // It is the same word in German, as expected.
            // I DO NOT GET THE SAME RESULTS FOR THIS COMPARISON USING JAVA!!!! O.o WTF!!! :(
            string[] strasse = {"strasse", "straße" };
            Console.WriteLine("strasse ");
            int result = String.Compare(strasse[0], strasse[1], CultureInfo.CreateSpecificCulture("de-DE"), CompareOptions.IgnoreCase);
            Console.WriteLine("German result: {0}", result);
            // And also in Spanish... why in Spanish is the same word? O.o
            result = String.Compare(strasse[0], strasse[1], CultureInfo.GetCultureInfo("es-ES"), CompareOptions.IgnoreCase);
            Console.WriteLine("Spanish result: {0}", result);

            // Shouldn't it be the same word in German?
            // IN THIS CASE I GET THE SAME RESULTS USING JAVA :)
            string[] koennen = {"können", "koennen" };
            Console.WriteLine("koennen ");
            result = String.Compare(koennen[0], koennen[1], CultureInfo.CreateSpecificCulture("de-DE"), CompareOptions.IgnoreCase);
            Console.WriteLine("German result: {0}", result);
            // Neither in German nor in Spanish they are the same word. I do not understand collations :(
            result = String.Compare(koennen[0], koennen[1], CultureInfo.GetCultureInfo("es-ES"), CompareOptions.IgnoreCase);
            Console.WriteLine("Spanish result: {0}", result);
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
