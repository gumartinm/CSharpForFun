using System;

namespace Chapter7
{
	partial class Example<TFirst, TSecond> : IEquatable<string> where TFirst : class
	{
		public bool Equals(string other)
		{
			return false;
		}
	}


	partial class PartialMethodDemo
	{
		public PartialMethodDemo ()
		{
			OnConstructorStart();
			Console.WriteLine("Generated constructor");
			OnConstructorEnd();
		}

		partial void OnConstructorStart();
		partial void OnConstructorEnd();
	}
}

