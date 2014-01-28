using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Chapter4
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/**
			 * Listing 4.1 Using various members of Nullable<T>
			 * 
			 */
			Console.WriteLine("Listing 4.1 Using various members of Nullable<T>");
			Nullable<int> x = 5;
			x = new Nullable<int>(5);
			Console.WriteLine("Instance with value:");
			Display(x);

			x = new Nullable<int>();
			Console.WriteLine("Instance without value:");
			Display(x);

			/**
			 * Listing 4.2 Boxing and unboxing behaviour of nullable types.
			 *
			 */
			Console.WriteLine("Listing 4.2 Boxing and unboxing behaviour of nullable types.");
			Nullable<int> nullable42 = 5;

			object boxed42 = nullable42;
			Console.WriteLine(boxed42.GetType());

			int normal = (int)boxed42;
			Console.WriteLine(normal);

			nullable42 = (Nullable<int>)boxed42;
			Console.WriteLine(nullable42);

			nullable42 = new Nullable<int>();
			boxed42 = nullable42;
			Console.WriteLine(boxed42 == null);

			nullable42 = (Nullable<int>)boxed42;
			Console.WriteLine(nullable42.HasValue);

			/**
			 * Listing 4.3 The same code as 4.2 but using the ? modifier.
			 *
			 */
			Console.WriteLine("Listing 4.3 The same code as 4.2 but using the ? modifier.");
			int? nullable43 = 5;

			object boxed43 = nullable43;
			Console.WriteLine(boxed43.GetType());

			int normal43 = (int)boxed43;
			Console.WriteLine(normal43);

			nullable43 = (Nullable<int>)boxed43;
			Console.WriteLine(nullable43);

			nullable43 = new Nullable<int>();
			boxed43 = nullable43;
			Console.WriteLine(boxed43 == null);

			nullable43 = (Nullable<int>)boxed43;
			Console.WriteLine(nullable43.HasValue);

			/**
			 * Listing 4.4 Part of a Person class including calculation of age.
			 *
			 */
			Console.WriteLine("Listing 4.4 Part of a Person class including calculation of age.");
			Person turing = new Person("Alan Turing ", new DateTime(1912, 6, 23), new DateTime(1954, 6, 7));
			Person knuth = new Person("Donald Knuth ", new DateTime(1938, 1, 10), null);
			Console.WriteLine(turing.Age.Days);
			Console.WriteLine(knuth.Age.Days);
			Console.WriteLine(turing.AgeCoalescing.Days);
			Console.WriteLine(knuth.AgeCoalescing.Days);

			/**
			 * Listing 4.5 An alternative implementation of th TryXXX patern.
			 *
			 */
			Console.WriteLine("Listing 4.5 An alternative implementation of th TryXXX patern.");
			int? parsed = TryParse("Not valid");
			if (parsed != null) {
				Console.WriteLine("Parsed to {0}", parsed.Value);
			} else {
				Console.WriteLine("Couldn't parse");
			}
		}

		static void Display(Nullable<int> x)
		{
			Console.WriteLine("HasValue: {0}", x.HasValue);
			if (x.HasValue) {
				Console.WriteLine("Value: {0}", x.Value);
				Console.WriteLine("Explicit conversion: {0}", (int)x);
			}
			Console.WriteLine("GetValueOrDefault(): {0}", x.GetValueOrDefault ());
			Console.WriteLine("GetValueOrDefault(10): {0}", x.GetValueOrDefault (10));
			Console.WriteLine("ToString(): \"{0}\"", x.ToString());
			Console.WriteLine("GetHashCode(): {0}", x.GetHashCode());
			Console.WriteLine();
		}

		class Person
		{
			DateTime birth;
			DateTime? death;
			string name;

			public TimeSpan Age
			{
				get {
					if (death == null) {
						return DateTime.Now - birth;
					} 
					else {
						return death.Value - birth;
					}
				}
			}

			public TimeSpan AgeCoalescing
			{
				get {
					DateTime lastAlive = death ?? DateTime.Now;
					return lastAlive - birth;
				}
			}

			public Person(string name, DateTime birth, DateTime? death)
			{
				this.birth = birth;
				this.death = death;
				this.name = name;
			}
		}

		static int? TryParse(string text)
		{
			int ret;
			if (int.TryParse(text, out ret)) {
				return ret;
			} 
			else {
				return null;
			}
		}
	}
}
