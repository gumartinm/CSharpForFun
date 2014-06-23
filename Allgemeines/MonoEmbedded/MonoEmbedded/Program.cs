using System;
using System.Runtime.CompilerServices;

namespace MonoEmbedded
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Print("characters like \u00D6 are working? (should be an O with ¨ above it)");
            Print("More characters: ÜÖüöÇç)");
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Print(string msg);
    }
}
