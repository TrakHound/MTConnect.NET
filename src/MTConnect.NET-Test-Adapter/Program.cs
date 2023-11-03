using MTConnect.Adapters.Shdr;

namespace MTConnect.NET_Test_Adapter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var adapter = new ShdrAdapter();
            adapter.Start();

            var i = 0;

            while (true)
            {
                adapter.SendDataItem("PartCountAct", i);
                //adapter.SendDataItem("Xpos", i);
                i++;

                if (i > 1000) break;

                Thread.Sleep(1);
            }

            Console.ReadLine();
        }
    }
}