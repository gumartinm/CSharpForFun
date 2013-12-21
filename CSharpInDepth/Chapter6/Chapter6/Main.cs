using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace Chapter6
{
	class MainClass
	{
		static readonly string Padding = new string(' ', 30);

		public static void Main (string[] args)
		{

			/**
			 * 
			 * Listings 6.1 and 6.2 Skeleton of th new collection type, with no iterator implementation.
			 */
			object[] values = {"a", "b", "c", "d", "e"};
			IterationSampleBad badCollection = new IterationSampleBad (values, 3);
			try {
				foreach (object x in badCollection) {
					Console.WriteLine (x);
				}
			} catch (NotImplementedException e) {
				Console.WriteLine ("Listings 6.1 and 6.2 exception: {0}", e);
			}


			/**
			 * 
			 * Listings 6.3 Nested class implementing the collection's iterator.
			 */
			IterationSample collection = new IterationSample (values, 3);
			foreach (object x in collection) {
				Console.WriteLine (x);
			}


			/**
			 * 
			 * Listings 6.4 Iterating through the sample collection with C# 2 and yield return.
			 */
			IterationSampleYield yieldCollection = new IterationSampleYield (values, 3);
			foreach (object x in yieldCollection) {
				Console.WriteLine (x);
			}


			/**
			 * 
			 * Listings 6.5 Showing the sequence of calls between an iterator and its caller.
			 */
			IEnumerable<int> iterable = CreateEnumerable ();
			IEnumerator<int> iterator = iterable.GetEnumerator ();
			Console.WriteLine ("Starting to iterate");
			while (true) {
				Console.WriteLine ("Calling MoveNext()...");
				bool result = iterator.MoveNext ();
				Console.WriteLine ("... MoveNext result={0}", result);
				if (!result) {
					break;
				}
				Console.WriteLine ("Fetching Current...");
				Console.WriteLine ("... Current result={0}", iterator.Current);
			}

			IEnumerable<string> stringIterable = CreateStringEnumerable ();
			IEnumerator<string> stringIterator = stringIterable.GetEnumerator ();
			Console.WriteLine ("Starting to iterate");
			while (true) {
				Console.WriteLine ("Calling MoveNext()...");
				bool result = stringIterator.MoveNext ();
				Console.WriteLine ("... MoveNext result={0}", result);
				if (!result) {
					break;
				}
				Console.WriteLine ("Fetching Current...");
				Console.WriteLine ("... Current result={0}", stringIterator.Current);
			}


			/**
			 * 
			 * Listings 6.6 Demonstration of yield break.
			 */
			DateTime stop = DateTime.Now.AddSeconds (2);
			foreach (int i in CountWithTimeLimit(stop)) {
				Console.WriteLine ("Received {0}", i);
				Thread.Sleep (300);
			}


			/**
			 * 
			 * Listings 6.8 Looping over the lines in a file using an iterator block.
			 */
			foreach (string line in ReadLines ("TheLayofLeithian.txt")) {
				Console.WriteLine(line);
			}


			/**
			 * 
			 * Listings 6.7 Demonstration of yield break working with try/finally.
			 */
			DateTime stopFinally = DateTime.Now.AddSeconds (2);
			foreach (int i in CountWithTimeLimitFinally(stopFinally)) {
				Console.WriteLine ("Received {0}", i);
				if (i > 3) {
					Console.WriteLine ("Returning");
					return;
				}
				Thread.Sleep (300);
			}
		}

		static IEnumerable<int> CreateEnumerable ()
		{
			Console.WriteLine ("{0}Start of CreateEnumerable()", Padding);

			for (int i=0; i < 3; i++) {
				Console.WriteLine("{0}About to yield {1}", Padding, i);
				yield return i;
				Console.WriteLine("{0}After yield", Padding);
			}

			Console.WriteLine("{0}Yielding final value", Padding);
			yield return -1;

			Console.WriteLine("{0}End of CreateEnumerable()", Padding);
		}

		static IEnumerable<string> CreateStringEnumerable ()
		{
			string[] values = {"a", "b", "c"};
			Console.WriteLine ("{0}Start of CreateStringEnumerable()", Padding);

			for (int i=0; i < 3; i++) {
				Console.WriteLine("{0}About to yield {1}", Padding, values[i]);
				yield return values[i];
				Console.WriteLine("{0}After yield", Padding);
			}

			Console.WriteLine("{0}Yielding final value", Padding);
			yield return null;

			Console.WriteLine("{0}End of CreateStringEnumerable()", Padding);
		}

		static IEnumerable<int> CountWithTimeLimit (DateTime limit)
		{
			for (int i = 1; i <= 100; i++) {
				if (DateTime.Now >= limit)
				{
					yield break;
				}
				yield return i;
			}
		}

		static IEnumerable<int> CountWithTimeLimitFinally (DateTime limit)
		{
			try {
				for (int i = 1; i <= 100; i++) {
					if (DateTime.Now >= limit) {
						yield break;
					}
					yield return i;
				}
			} finally {
				Console.WriteLine("Stopping!");
			}
		}

		static IEnumerable<string> ReadLines (string filename)
		{
			using (TextReader reader = File.OpenText(filename))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}
	}


	public class IterationSampleBad : IEnumerable
	{
		object[] values;
		int startingPoint;

		public IterationSampleBad (object[] values, int startingPoint)
		{
			this.values = values;
			this.startingPoint = startingPoint;
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}


	// The same as the Java style of creating a custom Iterator:
	// see http://stackoverflow.com/questions/5849154/can-we-write-our-own-iterator-in-java
	// (basically it is the same)
	public class IterationSample : IEnumerable
	{
		object[] values;
		int startingPoint;

		public IterationSample (object[] values, int startingPoint)
		{
			this.values = values;
			this.startingPoint = startingPoint;
		}

		public IEnumerator GetEnumerator()
		{
			// If GetEnumerator is called several times, serveral independent iterators should be returned.
			return new IterationSampleIterator(this);
		}

		// Let's create another nested class to implement the iterator itself.
		public class IterationSampleIterator : IEnumerator
		{
			IterationSample parent;
			// Iterators are stateful. We need to store some state somewhere (the position in this example)
			int position;

			internal IterationSampleIterator (IterationSample parent)
			{
				this.parent = parent;
				position = -1;
			}

			public bool MoveNext ()
			{
				if (position != parent.values.Length) {
					position++;
				}
				return position < parent.values.Length;
			}

			public object Current {
				get {
					if (position == -1 ||
				    	position == parent.values.Length) {
							throw new InvalidOperationException();
					}
					int index = position + parent.startingPoint;
					index = index % parent.values.Length;
					return parent.values[index];
				}
			}

			public void Reset()
			{
				position = -1;
			}
		}
	}


	// The C# 2.0 style! Rocks!
	public class IterationSampleYield : IEnumerable
	{
		object[] values;
		int startingPoint;

		public IterationSampleYield (object[] values, int startingPoint)
		{
			this.values = values;
			this.startingPoint = startingPoint;
		}

		public IEnumerator GetEnumerator ()
		{
			// * Whenever MoveNext is called, it has to execute code from the GetEnumerator
			//     method until you are ready to provide the next value (in other words, until you
			//     hit a yield return statement)
			// * When the Current property is used, it has to return the last value you yielded.
			// * It has to know when you have finished yielding values so that MoveNext can return false.
			for (int index = 0; index < values.Length; index++) {
				yield return values[(index + startingPoint) % values.Length];
			}
		}
	}
}
