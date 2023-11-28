using MTConnect.Observations;
using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using MTConnect.Assets.CuttingTools.Measurements;
using MTConnect.Assets.CuttingTools;

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
            var j = 0d;

            while (true)
            {
                Console.ReadLine();

                var rnd = new Random();
                var ts = DateTime.Now;

                app.DataSource.AddObservation("Xload", x, ts);
                app.DataSource.AddObservation("Yload", x, ts);
                app.DataSource.AddObservation("Zload", x, ts);


                var datasetEntries = new List<IDataSetEntry>();
                for (var e = 0; e < 100; e++)
                {
                    datasetEntries.Add(new DataSetEntry($"E{e.ToString("D3")}", rnd.NextDouble() * 100));
                }           
                var dataset = new DataSetObservationInput("testDataSet", datasetEntries);
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

                var table = new TableObservationInput("testTable", tableEntries);
                app.DataSource.AddObservation(table);


                var samples = new List<double>();
                samples.Add(1);
                samples.Add(2);
                samples.Add(3);
                samples.Add(4);
                samples.Add(5);
                var timeSeries = new TimeSeriesObservationInput("testTimeSeries", samples, 100);
                app.DataSource.AddObservation(timeSeries);


                app.DataSource.AddAsset(CuttingTool(j));
                j += 23.3455;


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

        public static CuttingToolAsset CuttingTool(double toolLifeValue)
        {
            var tool = new CuttingToolAsset();
            tool.SerialNumber = "12345678946";
            tool.AssetId = "5.12";
            tool.ToolId = "12";

            var cuttingToolLifeCycle = new CuttingToolLifeCycle
            {
                //Location = new Location
                //{
                //    Type = LocationType.SPINDLE
                //},
                ProgramToolNumber = "12",
                ProgramToolGroup = "5"
            };

            var measurements = new List<IMeasurement>();
            measurements.Add(new FunctionalLengthMeasurement(7.6543));
            measurements.Add(new CuttingDiameterMaxMeasurement(0.375));
            cuttingToolLifeCycle.Measurements = measurements;


            //tool.CuttingToolLifeCycle.CuttingItems.Add(new CuttingItem
            //{
            //    ItemId = "12.1",
            //    Indices = "1",
            //});

            var cutterStatuses = new List<CutterStatusType>();
            cutterStatuses.Add(CutterStatusType.AVAILABLE);
            cutterStatuses.Add(CutterStatusType.NEW);
            cutterStatuses.Add(CutterStatusType.MEASURED);
            cuttingToolLifeCycle.CutterStatus = cutterStatuses;

            var toolLife = new ToolLife();
            toolLife.Value = toolLifeValue;
            cuttingToolLifeCycle.ToolLife = new IToolLife[] { toolLife };

            //tool.CuttingToolLifeCycle.ToolLife = new ToolLife();

            tool.CuttingToolLifeCycle = cuttingToolLifeCycle;

            tool.Timestamp = DateTime.Now;

            return tool;
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
