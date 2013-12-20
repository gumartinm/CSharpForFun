using System;
using Gtk;
using System.IO;
using System.Collections.Generic;

namespace Chapter5
{
	class MainClass
	{
		delegate Stream StreamFactory();
		delegate void SampleDelegate(string x);
		delegate void MethodInvoker();

		public static void Main (string[] args)
		{
			Application.Init ();

			MainWindow win = new MainWindow ();


			/**
			 * 
			 * Listing 5.1 Subscribing to three of a button's events. 
			 */
			Button buttonA = new Button ("Click me 5.1");
			buttonA.Pressed += new EventHandler (LogPlainEvent);
			buttonA.KeyPressEvent += new KeyPressEventHandler (LogKeyEvent);
			win.Add (buttonA);


			/**
			 * 
			 * Listing 5.2 Demonstration of method group conversion and delegate contravariance. 
			 */
			Button buttonB = new Button ("Click me 5.2");
			buttonB.Pressed += LogPlainEvent;
			buttonB.KeyPressEvent += LogPlainEvent;
			win.Add (buttonB);


			/**
			 * 
			 * Listing 5.3 Demonstration of covariance of return types for delegates. 
			 */
			StreamFactory factoryA = GenerateSampleData;

			using (Stream stream = factoryA()) {
				int data;

				while ((data = stream.ReadByte()) != -1) {
					Console.WriteLine (data);
				}
			}


			/**
			 * 
			 * Listing 5.4 Demonstration of braking change between C# 1 and C# 2. 
			 */
			Derived x = new Derived ();
			SampleDelegate factoryB = new SampleDelegate (x.CandidateAction);
			factoryB ("test");


			/**
			 * 
			 * Listing 5.5 Anonymous methods used with the Action<T> delegate type.
			 */
			Action<string> printReverse = delegate(string text) {
				char[] chars = text.ToCharArray ();
				Array.Reverse (chars);
				Console.WriteLine (new String (chars));
			};

			Action<int> printRoot = delegate(int number) {
				Console.WriteLine (Math.Sqrt (number));
			};

			Action<IList<double>> printMean = delegate(IList<double> numbers) {
				double total = 0;
				foreach (double value in numbers) {
					total += value;
				}
				Console.WriteLine (total / numbers.Count);
			};

			printReverse ("Hello Gus");
			printRoot (2);
			printMean (new double[] {1.5, 2.5, 3.5, 4.5, 5, 7, 10.1});


			/**
			 * 
			 * Listing 5.7 Returning a value from an anonymous method.
			 */
			Predicate<int> isEven = delegate(int number) {
				return number % 2 == 0;
			};
			Console.WriteLine (isEven (1));
			Console.WriteLine (isEven (4));


			/**
			 * 
			 * Listing 5.8 Using anonymous methods to sort files simply.
			 */
			SortAndShowFiles ("Sorted by name:", delegate(FileInfo f1, FileInfo f2) {
				return f1.Name.CompareTo (f2.Name);
			}
			);

			SortAndShowFiles ("Sorted by length:", delegate(FileInfo f1, FileInfo f2) {
				return f1.Length.CompareTo (f2.Length);
			}
			);


			/**
			 * 
			 * Listing 5.9 Subscribing to event with anonymous methods that ignore parameters.
			 */
			Button buttonC = new Button ("Click me 5.9");
			buttonC.Pressed += delegate {
				Console.WriteLine ("LogPlain");
			};
			buttonC.KeyPressEvent += delegate {
				Console.WriteLine ("LogKey");
			};
			win.Add (buttonC);


			/**
			 * 
			 * Listing 5.10 Examples of variable kinds with respect to anonymous methods.
			 */
			int outerVariable = 5;
			string capturedVariable = "captured";

			if (DateTime.Now.Hour == 1) {
				int normalLocalVariable = DateTime.Now.Minute;
				Console.WriteLine (normalLocalVariable);
			}

			MethodInvoker method = delegate() {
				string anonLocal = " local to anonymous method";
				Console.WriteLine (capturedVariable + anonLocal);
			};
			method ();


			/**
			 * 
			 * Listing 5.11 Accessing a variable both inside and outside an anonymous method.
			 */
			string captured = "before methodB is created";

			MethodInvoker methodB = delegate {
				Console.WriteLine (captured);
				captured = "changed by methodB";
			};
			captured = "directly before methodB is invoked";
			methodB ();

			Console.WriteLine (captured);

			captured = "before second invocation";
			methodB ();


			/**
			 * 
			 * Listing 5.12 Demonstration of a captured variable having its lifetime extended.
			 */
			MethodInvoker methodC = CreateDelegateInstance ();
			methodC ();
			methodC ();


			/**
			 * 
			 * Listing 5.13 Capturing multiple variable instantiations with multiple delegates.
			 */
			List<MethodInvoker> list = new List<MethodInvoker> ();

			// The variable declared by the initial part of the loop is only instantiated once.
			for (int index = 0; index < 5; index++) {
				// Because counter is declared inside the loop, it's instantiated for each iteration.
				int counter = index * 10;
				list.Add (delegate {
					Console.WriteLine (counter);
					counter++;
				}
				);
			}

			foreach (MethodInvoker t in list) {
				t ();
			}
			list [0] ();
			list [0] ();
			list [0] ();

			list [1] ();


			/**
			 * 
			 * Listing 5.14 Capturing variables in different scopes. Warning: nasty code ahead!
			 */
			MethodInvoker[] delegates = new MethodInvoker[2];

			int outside = 0;

			for (int i = 0; i < 2; i++) {
				// Because inside is declared inside the loop, it's instantiated for each iteration.
				int inside = 0;

				delegates[i] = delegate {
					Console.WriteLine ("({0},{1})", outside, inside);
					outside++;
					inside++;
				};
			}

			MethodInvoker first = delegates[0];
			MethodInvoker second = delegates[1];

			first();
			first();
			first();

			second();
			second();



			win.ShowAll ();
			Application.Run ();
		}


		static void LogPlainEvent (object sender, EventArgs e)
		{
			Console.WriteLine("LogPlain");
		}


		static void LogKeyEvent (object sender, KeyPressEventArgs e)
		{
			Console.WriteLine("LogKey");
		}


		static MemoryStream GenerateSampleData ()
		{
			byte[] buffer = new byte[16];
			for (int i = 0; i < buffer.Length; i++) {
				buffer[i] = (byte) i;
			}

			return new MemoryStream(buffer);
		}


		public void CandidateAction(string x)
		{
			Console.WriteLine("Snippet.CandidateAction");
		}


		public class Derived:MainClass
		{
			public void CandidateAction(object o)
			{
				Console.WriteLine("Derived.CandidateAction");
			}
		}

		static void SortAndShowFiles (string title, Comparison<FileInfo> sortOrder)
		{
			FileInfo[] files = new DirectoryInfo (@"/").GetFiles ();

			Array.Sort (files, sortOrder);

			Console.WriteLine (title);
			foreach (FileInfo file in files) {
				Console.WriteLine(" {0} ({1} bytes)", file.Name, file.Length);
			}
		}

		static MethodInvoker CreateDelegateInstance ()
		{
			int counter = 5;

			MethodInvoker ret = delegate
			{
				Console.WriteLine(counter);
				counter++;
			};

			ret();
			return ret;
		}
	}

}
