using MTConnect.Observations;
using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MTConnect.Applications
{
    public class Program
    {
        // This is the Application Name shown in the Console header information
        // If you are implementing this into your own application, you can change this to be more specific (ex. Fanuc MTConnect Adapter, Mazak MTConnect Adapter, etc.)
        private const string ApplicationName = "MTConnect Adapter";

        // Copyright statement for the application. If you are implementing this into your own application, you can change this to your own copyright, or set it to 'null'.
        // This is just what is shown in the console header.
        private const string ApplicationCopyright = "Copyright 2023";

        public static void Main(string[] args)
        {
            // Print an application header to the console
            PrintConsoleHeader();

            var dataSource = new PlcDataSource();

            // Create a new MTConnect Adapter Application
            var app = new MTConnectAdapterApplication(dataSource);

            // Run the Agent ('true' parameter blocks the call so the application does not continue)
            //app.Run(args, true);








            // DEBUG !!!
            app.Run(args, false);

            var x = 0;
            var i = 0;

            while (true)
            {
                Console.ReadLine();

                var ts = DateTime.Now;

                app.DataSource.AddObservation("L2X1load", x, ts);
                app.DataSource.AddObservation("L2Y1load", x, ts);
                app.DataSource.AddObservation("L2Z1load", x, ts);


                var datasetEntries = new List<IDataSetEntry>();
                datasetEntries.Add(new DataSetEntry("E100", 123.456));
                datasetEntries.Add(new DataSetEntry("E101", 45));
                datasetEntries.Add(new DataSetEntry("E102", 78));
                var dataset = new DataSetObservationInput("L2p1Variables", datasetEntries);
                app.DataSource.AddObservation(dataset);


                var tableEntries = new List<ITableEntry>();

                var tableCells1 = new List<ITableCell>();
                tableCells1.Add(new TableCell("Length", 102.310));
                tableCells1.Add(new TableCell("Diameter", 12.493));
                tableEntries.Add(new TableEntry("T1", tableCells1));

                var tableCells2 = new List<ITableCell>();
                tableCells2.Add(new TableCell("Length", 128.942));
                tableCells2.Add(new TableCell("Diameter", 6.500));
                tableEntries.Add(new TableEntry("T2", tableCells2));

                var table = new TableObservationInput("L2p1ToolTable", tableEntries);
                app.DataSource.AddObservation(table);


                var samples = new List<double>();
                samples.Add(1);
                samples.Add(2);
                samples.Add(3);
                samples.Add(4);
                samples.Add(5);
                var timeSeries = new TimeSeriesObservationInput("L2p1Sensor", samples, 100);
                app.DataSource.AddObservation(timeSeries);


                //switch (i)
                //{
                //    case 0:
                //        var warning = new ConditionFaultStateObservationInput("L2p1system", Observations.ConditionLevel.WARNING, ts);
                //        warning.NativeCode = "404";
                //        warning.Message = "Not Found";
                //        warning.Qualifier = Observations.ConditionQualifier.LOW;
                //        app.DataSource.AddObservation(warning);
                //        break;

                //    case 1:
                //        var fault = new ConditionFaultStateObservationInput("L2p1system", Observations.ConditionLevel.FAULT, ts);
                //        fault.NativeCode = "400";
                //        fault.Message = "Bad Request";
                //        app.DataSource.AddObservation(fault);
                //        break;

                //    case 2:
                //        var condition = new ConditionFaultStateObservationInput("L2p1system", Observations.ConditionLevel.NORMAL, ts);
                //        app.DataSource.AddObservation(condition);
                //        break;
                //}

                x++;
                i++;
                if (i > 2) i = 0;
            }

        }


        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            if (!string.IsNullOrEmpty(ApplicationCopyright)) Console.WriteLine(ApplicationCopyright);
            Console.WriteLine(ApplicationName + " : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the MIT License (https://choosealicense.com/licenses/mit/)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}
