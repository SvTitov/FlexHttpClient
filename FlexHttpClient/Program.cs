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
                FlexClient client = new FlexClient("www.openlibrary.org");
                var result = await client.Get("api/books?bibkeys=ISBN:0201558025,LCCN:93005405");
            });

            Console.ReadKey(true);
        }
    }
}
