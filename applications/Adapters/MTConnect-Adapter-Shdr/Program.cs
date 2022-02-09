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
            _adapterExample1.Start();
            Console.ReadLine();

            while (true)
            {
                _adapterExample1.UpdateTemperature1();
                Console.ReadLine();

                _adapterExample1.UpdateTemperature2();
                Console.ReadLine();

                //_adapterExample1.UpdateDataSet1();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet2();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet3();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet4();
                //Console.ReadLine();

                //_adapterExample1.UpdateTest1();
                //Console.ReadLine();

                //_adapterExample1.UpdateTest2();
                //Console.ReadLine();

                //_adapterExample1.UpdateTest3();
                //Console.ReadLine();
            }

            //while (true)
            //{
            //    _adapterExample1.Start();

            //    Console.ReadLine();
            //    _adapterExample1.Stop();

            //    Console.ReadLine();
            //}

            Console.ReadLine();
        }
    }
}
