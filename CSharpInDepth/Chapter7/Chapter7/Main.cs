using System;

namespace Chapter7
{
	class Chapter7
	{
		public static void Main (string[] args)
		{

			/**
			 *
			 * Listing 7.1 Demonstration of mixing declarations of a partial type.
			 */
			Console.WriteLine("Listing 7.1: Demonstration of mixing declarations of a partial type.");
			Example<string, int> example = new Example<string, int>();
			example.Dispose();


			/**
			 *
			 * Listing 7.2 A partial method called from a constructor.
			 */
			Console.WriteLine("Listing 7.2: A partial method called from a constructor.");
#pragma warning disable 0219
			PartialMethodDemo partialMethodDemo = new PartialMethodDemo();
#pragma warning restore 0219

			/**
			 *
			 * Listing 7.3 A typical C# 1 utility class.
			 */
			Console.WriteLine("Listing 7.3: A typical C# 1 utility class.");
			string message = "noimahcrE nereB";
			string reverseMessage = NonStaticStringHelper.Reverse(message);
			Console.WriteLine(reverseMessage);


			/**
			 *
			 * Listing 7.4 The same utility class as in listing 7.3, but converted a C# 2 static class.
			 */
			Console.WriteLine("Listing 7.4: The same utility class as in listing 7.3, but converted a C# 2 static class.oimahcrE nereB");
			message = "leivúniT neihtúL";
			reverseMessage = NonStaticStringHelper.Reverse(message);
			Console.WriteLine(reverseMessage);

		}
	}

	public sealed class NonStaticStringHelper
	{
		private NonStaticStringHelper ()
		{
		}

		public static string Reverse(string input)
		{
			char[] chars = input.ToCharArray();
			Array.Reverse(chars);
			return new string(chars);
		}
	}


	public static class StringHelper
	{
		public static string Reverse(string input)
		{
			char[] chars = input.ToCharArray();
			Array.Reverse(chars);
			return new string(chars);
		}
	}
}
