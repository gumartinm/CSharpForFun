using System;

namespace Chapter7
{
	partial class Example<TFirst, TSecond> : EventArgs, IDisposable
	{
		public void Dispose()
		{
			Console.WriteLine("Running Dispose");
		}
	}


	partial class PartialMethodDemo
	{
		partial void OnConstructorEnd()
		{
			Console.WriteLine("Manual code");
		}
	}
}

