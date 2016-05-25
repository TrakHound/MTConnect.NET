using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MTConnect;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Address of the demo agent provided by MTConnect
            string url = "http://agent.mtconnect.org";

            // Get Probe Request
            PrintProbeDataItems(url);

            Console.ReadLine();
        }

        private static void PrintProbeDataItems(string url)
        {
            var data = MTConnect.Components.Requests.Get(url);
            if (data != null)
            {
                foreach (var device in data.Devices)
                {
                    var dataItems = device.GetAllDataItems();

                    foreach (var dataItem in dataItems)
                    {
                        string format = "Id:{0}; Name:{1}; Category:{2}; Type:{3}; SubType:{4};";

                        string info = string.Format(format,
                            dataItem.Id,
                            dataItem.Name,
                            dataItem.Category.ToString(),
                            dataItem.Type,
                            dataItem.SubType);

                        Console.WriteLine(info);
                    }
                }
            }
        }
    }
}
