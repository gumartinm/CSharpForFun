using System;
using System.Collections.Generic;

namespace Chapter8
{
	class Chapter8
	{
		public static void Main (string[] args)
		{

			/**
			 * 
			 * Listing 8.1 Counting instances awkwardly with a static automatic property.
			 */
			Console.WriteLine ("Listing 8.1: Counting instances awkwardly with a static automatic property.");
			Person person = new Person ("Beren", 71);
			Console.WriteLine ("Name: {0}, Age: {1}", person.Name, person.Age);


			/**
			 * 
			 * Listing 8.2 A fairly simple Person class used for further demonstrations.
			 */
			Console.WriteLine ("Listing 8.2: A fairly simple Person class used for further demonstrations.");
			NewPerson beor = new NewPerson ();
			beor.Name = "Bëor";
			beor.Age = 93;

			NewPerson beor2 = new NewPerson ("Bëor");
			beor2.Age = 93;

			NewPerson beor3 = new NewPerson () { Name = "Bëor", Age = 93 };
			NewPerson beor4 = new NewPerson { Name = "Bëor", Age = 93 };
			NewPerson beor5 = new NewPerson ("Bëor") { Age = 93 };

			NewPerson[] houseOfBeor = new NewPerson[]
			{
				new NewPerson { Name = "Baran", Age = 91 },
				new NewPerson { Name = "Boron", Age = 93 },
				new NewPerson { Name = "Boromir", Age = 94 },
				new NewPerson { Name = "Bregor", Age = 62 },
				new NewPerson { Name = "Barahir", Age = 60 }
			};

			NewPerson beor6 = new NewPerson ("Bëor");
			beor6.Age = 93;
			beor6.Home.Country = "Estolad";
			beor6.Home.Town = "Estolad";

			NewPerson beor7 = new NewPerson ("Bëor")
			{
				Age = 93,
				Home = { Country = "Estolad", Town = "Estolad" }
			};


			/**
			 * 
			 * Listing 8.3 Building up a rich object using object and collection initializers.
			 */
			Console.WriteLine ("Listing 8.3: Building up a rich object using object and collection initializers.");
			NewPerson beor8 = new NewPerson
			{
				Name = "Bëor",
				Age = 93,
				Home = { Town = "Estolad", Country = "Estolad" },
				Friends = 
				{
					new NewPerson { Name = "Baran" },
					new NewPerson("Boron"),
					new NewPerson { Name = "Boromir", Age = 94 }
				}
			};


			/**
			 * 
			 * Listing 8.4 Creating objects of an anonymous type with Name and Age properties.
			 */
			Console.WriteLine ("Listing 8.4: Creating objects of an anonymous type with Name and Age properties.");
			var finwe = new { Name = "Finwë", Age = 4293 };
			var feanor = new { Name = "Fëanor", Age = 3142 };
			var fingolfin = new { Name = "Fingolfin", Age = 3426 };
			Console.WriteLine ("{0} was {1} years old when he died", finwe.Name, finwe.Age);
			Console.WriteLine ("{0} was {1} years old when he died", feanor.Name, feanor.Age);
			Console.WriteLine ("{0} was {1} years old when he died", fingolfin.Name, fingolfin.Age);


			/**
			 * 
			 * Listing 8.5 Populating an array using anonymous types and then finding the total age.
			 */
			Console.WriteLine ("Listing 8.5: Populating an array using anonymous types and then finding the total age.");
			var houseOfBeor2 = new []
			{
				new { Name = "Baran", Age = 91 },
				new { Name = "Boron", Age = 93 },
				new { Name = "Boromir", Age = 94 },
				new { Name = "Bregor", Age = 62 },
				new { Name = "Barahir", Age = 60 }
			};

			int totalAge = 0;
			foreach (var man in houseOfBeor2) {
				totalAge += man.Age;
			}

			Console.WriteLine ("Total age: {0}", totalAge);


			/**
			 * 
			 * Listing 8.6 Transformation from Person to a name and adulthood flag.
			 */
			Console.WriteLine ("Listing 8.6: Transformation from Person to a name and adulthood flag.");
			List<NewPerson> houseOfBeor3 = new List<NewPerson>
			{
				new NewPerson { Name = "Baran", Age = 91 },
				new NewPerson { Name = "Boron", Age = 93 },
				new NewPerson { Name = "Boromir", Age = 94 },
				new NewPerson { Name = "Bregor", Age = 62 },
				new NewPerson { Name = "Barahir", Age = 60 }
			};
			var converted = houseOfBeor3.ConvertAll (delegate(NewPerson man) 
			                       { return new { man.Name, IsAdult = (man.Age >= 18) };}
			);
			foreach (var man in converted)
			{
				Console.WriteLine("{0} is an adult? {1}", man.Name, man.IsAdult);
			}
		}
	}

	public class Person
	{
		public string Name { get; private set; }
		public int Age { get; private set; }

		private static int InstanceCounter { get; set; }
		private static readonly object counterLock = new object();

		public Person (string name, int age)
		{
			Name = name;
			Age = age;
			lock (counterLock) {
				InstanceCounter++;
			}
		}
	}

	public class NewPerson
	{
		public int Age { get; set; }
		public string Name { get; set; }

		List<NewPerson> friends = new List<NewPerson>();
		public List<NewPerson> Friends { get { return friends; } }

		Location home = new Location();
		public Location Home { get { return home; } }

		public NewPerson () { }

		public NewPerson (string name)
		{
			Name = name;
		}
	}

	public class Location
	{
		public string Country { get; set; }
		public string Town { get; set; }
	}
}
