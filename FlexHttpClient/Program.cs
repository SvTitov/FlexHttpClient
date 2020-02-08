using System;
using System.Threading.Tasks;
using FlexHttpClient.Core.Basic;

namespace FlexHttpClient
{
    class Program
    {
        public static object FlexClient { get; private set; }

        static void Main(string[] args)
        {
            Task.Factory.StartNew(async () =>
            {
                FlexClient client = new FlexClient("www.google.com");
                var result = await client.Get();
            });

            Console.ReadKey(true);
        }
    }
}
