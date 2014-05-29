using System;

namespace HttpClientsExamples
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            WebClientExample webclientExample = new WebClientExample();
            webclientExample.Test();
            HttpWebRequestExample httpWebRequestExample = new HttpWebRequestExample();
            httpWebRequestExample.Test();
        }
    }
}
