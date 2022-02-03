using MTConnect.Streams;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using MTConnect.Observations;
using MTConnect.Adapters.Shdr;

namespace MTConnect.Applications.Adapters.Shdr
{
    class Program
    {
        static Adapters.AdapterExample1 _adapterExample1;


        static async Task Main(string[] args)
        {
            _adapterExample1 = new Adapters.AdapterExample1("M12346");

            while (true)
            {
                _adapterExample1.Start();

                Console.ReadLine();
                _adapterExample1.Stop();

                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}
