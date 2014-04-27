using System;
using System.Globalization;

namespace Decimal
{
    class MainClass
    {
        // Like with BigDecimal in Java if DECIMALS ARE IMPORTANT FOR YOU NEVER USE DOUBLE OR FLOAT
        // YOU MUST USE DECIMAL IN THE WHOLE APPLICATION.

        // C# IS NICER THAN JAVA IN ALMOST EVERY ASPECT!!!! COMPARE THIS TO MY BigDecimal EXAMPLES :(

        public static void Main(string[] args)
        {
            // With decimal, C# tries to use the decimals as you see them, without "loosing" values because of the IEE 754
            // It would be like the new BigDecimal(String) for Java being String = "165.01499999999998"
            decimal fromDecimal = 165.01499999999998m;
            string fromDecimalToString = fromDecimal.ToString("G", CultureInfo.InvariantCulture);
            Console.WriteLine("fromDecimal: {0}", fromDecimalToString);
            // 165.01499999999998
            // SCALE: 4 165.0149
            // AWAYFROMZERO (HALF UP BigDecimal Java): 0.9999999998 more than 0.5 then we should see as result 165.0150
            decimal fromDecimalRoundFour = Math.Round(fromDecimal, 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("fromDecimalRound four: {0}", fromDecimalRoundFour);
            // 165.01499999999998
            // SCALE: 2 165.01
            // AWAYFROMZERO (HALF UP BigDecimal Java): 0.499999999998 less than 0.5 then we should see as result 165.01
            decimal fromDecimalRoundTwo = Math.Round(fromDecimal, 2, MidpointRounding.AwayFromZero);
            Console.WriteLine("fromDecimalRound two: {0}", fromDecimalRoundTwo);

            // In the edges of your application you could have for example prices as strings
            string stringDecimal = "165.01499999999998";
            // We could convert prices to decimal and work always with decimal for currency :)
            decimal fromString = decimal.Parse(stringDecimal, NumberStyles.Float, CultureInfo.InvariantCulture);
            string fromStringToString = fromString.ToString("G", CultureInfo.InvariantCulture);
            Console.WriteLine("fromString: {0}", fromStringToString);


            decimal fromShortDecimal = 165.015m;
            string fromShortDecimalToString = fromShortDecimal.ToString("G", CultureInfo.InvariantCulture);
            Console.WriteLine("fromShortDecimal: {0}", fromShortDecimalToString);

            // With doubles the things begin to be weird. As usual because the in memory value is not what you expect.
            // For example: monodis Decimal/Decimal/bin/Debug/Decimal.exe
            // In CIL we see: 165.01499999999979 We are already loosing some decimals.
            // THIS IS WORSE THAN javac. WITH JAVA WHEN I WRITE double=165.0149999999998 I GET IN BYTECODE 165.015.
            // The in memory value (IEE 754) for 165.0149999999998 and 165.015 is the same: 165.0149999999999863575794734060764312744140625
            // BUT WITH MONO I GET IN CIL: 165.0149999999997 AND ITS IN MEMORY VALUE IS DIFFERENT TO 165.0149999999998
            // OMG THIS COMPLETELY SUCKS!!!!! :/
            double fromDouble = 165.0149999999998;  // IN CIL I HAVE 165.01499999999979, THE IN MEMORY VALUE IS DIFFERENT!!! SO, WE
                                                    // COULD FINISH HAVING A DIFFERENT RESULT!!!! WITH JAVAC I GOT 165.015 IN BYTECODE :(
                                                    // IT IS WORS THAN javac :(
            Console.WriteLine("fromDouble: {0}", fromDouble);


            // AS ALWAYS: ARE DECIMALS IMPORTANT FOR YOU? IF YES, YOU HAVE TO USE decimal
            // WHEN DO YOU KNOW IF DECIMALS ARE IMPORTANT FOR YOU? IT DEPENDS...
            // see: my example of BigDecimal in Java project, there I "try" to explain how to know when you have
            // have to use fixed-point numbers.
        }
    }
}
