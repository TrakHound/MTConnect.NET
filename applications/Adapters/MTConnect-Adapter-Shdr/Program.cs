//using MTConnect.Streams;
//using MTConnect.Streams.Events;
//using MTConnect.Streams.Samples;
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

            _adapterExample1.AddDevice();
            Console.ReadLine();

            while (true)
            {
                //_adapterExample1.RemoveAsset();
                //Console.ReadLine();

                //_adapterExample1.RemoveAllAssets();
                //Console.ReadLine();




                _adapterExample1.UpdateValue();
                Console.ReadLine();

                _adapterExample1.UpdateTest56();
                Console.ReadLine();

                //_adapterExample1.AddCuttingTools();
                //Console.ReadLine();

                //_adapterExample1.UpdateUnavaiableTest();
                //Console.ReadLine();

                //_adapterExample1.SetUnavailable();
                //Console.ReadLine();

                //_adapterExample1.AddCuttingTools();
                //Console.ReadLine();

                //_adapterExample1.UpdateTestTable1();
                //Console.ReadLine();

                //_adapterExample1.UpdateTestTable2();
                //Console.ReadLine();

                //_adapterExample1.UpdateTestTable3();
                //Console.ReadLine();

                //_adapterExample1.Update();
                //Console.ReadLine();

                //_adapterExample1.UpdateTemperature1();
                //Console.ReadLine();

                //_adapterExample1.UpdateTemperature2();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet1();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet2();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet3();
                //Console.ReadLine();

                //_adapterExample1.UpdateDataSet4();
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

                await Task.Delay(10);
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
