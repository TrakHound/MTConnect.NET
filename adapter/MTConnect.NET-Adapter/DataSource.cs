using MTConnect.Input;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTConnect.Applications
{
    // This class is used for reading from your DataSource (ex. PLC) and writing to the "Adapter" object that is included in the base class

    // OnRead() : Gets called at the specfied Interval in the Configuration and would be where you can scan the PLC variables that you want to write to the MTConnect Agent

    // OnReadAsync() : Same as OnRead() but is an async method


    internal class DataSource : MTConnectDataSource
    {
        //protected readonly Logger _engineLogger = LogManager.GetLogger("engine-logger");
        //private PlcSimulator _dataSource;
        //private CuttingToolAsset asset = Examples.CuttingTool();


        protected override void OnStart()
        {
            //_engineLogger.Info($"Connected to PLC @ {Configuration.PlcAddress} : Port = {Configuration.PlcPort}");

            //_dataSource = new PlcSimulator(Configuration.PlcAddress, Configuration.PlcPort, 10);
            //_dataSource.Connect();
        }

        protected override void OnStop()
        {
            //Client.SetUnavailable();

            //_dataSource.Disconnect();

            //_engineLogger.Info($"Disconnected from PLC @ {Configuration.PlcAddress} : Port = {Configuration.PlcPort}");
        }

        protected override void OnRead()
        {
            var x = 0;
            var i = 0;
            var j = 0d;

            while (true)
            {
                //Console.ReadLine();

                var rnd = new Random();
                var ts = DateTime.Now;

                AddObservation("Xload", x, ts);
                AddObservation("Yload", x, ts);
                AddObservation("Zload", x, ts);


                var datasetEntries = new List<IDataSetEntry>();
                for (var e = 0; e < 100; e++)
                {
                    datasetEntries.Add(new DataSetEntry($"E{e.ToString("D3")}", rnd.NextDouble() * 100));
                }
                var dataset = new DataSetObservationInput("testDataSet", datasetEntries);
                AddObservation(dataset);


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
                AddObservation(table);


                var samples = new List<double>();
                samples.Add(1);
                samples.Add(2);
                samples.Add(3);
                samples.Add(4);
                samples.Add(5);
                var timeSeries = new TimeSeriesObservationInput("testTimeSeries", samples, 100);
                AddObservation(timeSeries);


                //app.DataSource.AddAsset(CuttingTool(device.Uuid, j));
                j += 23.3455;




                //Console.ReadLine();

                //var device2 = XmlDevice.FromXml(System.IO.File.ReadAllBytes(@"D:\TrakHound\Source-Code\MTConnect.NET\agent\MTConnect.NET-Agent\bin\Debug\net8.0\devices\device-okuma.xml"));
                //app.DataSource.AddDevice(device2);


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

                Thread.Sleep(1000);
            }



            // Using a single Timestamp (per OnRead() call) can consolidate the SHDR output as well as make MTConnect data more "aligned" and easier to process
            //var ts = UnixDateTime.Now;
            //AddObservation("L2estop", EmergencyStop.ARMED, ts);

            //Adapter.AddDataItem("avail", _dataSource.Connected ? Availability.AVAILABLE : Availability.UNAVAILABLE, ts);

            //Adapter.AddDataItem("estop", _dataSource.EmergencyStop ? EmergencyStop.ARMED : EmergencyStop.TRIGGERED, ts);

            //switch (_dataSource.Mode)
            //{
            //    case 0: Adapter.AddDataItem("mode", ControllerMode.MANUAL, ts); break;
            //    case 1: Adapter.AddDataItem("mode", ControllerMode.SEMI_AUTOMATIC, ts); break;
            //    case 2: Adapter.AddDataItem("mode", ControllerMode.AUTOMATIC, ts); break;
            //    case 3: Adapter.AddDataItem("mode", ControllerMode.EDIT, ts); break;
            //}

            //Adapter.AddDataItem("program", _dataSource.ProcessDatas[0].Program, ts);
            //Adapter.AddDataItem("tool", _dataSource.ProcessDatas[0].ToolNumber, ts);
            //Adapter.AddDataItem("tool_offset", _dataSource.ProcessDatas[0].ToolOffset, ts);

            //for (var i = 0; i < _dataSource.AxisDatas.Length; i++)
            //{
            //    var axis = _dataSource.AxisDatas[i];
            //    Adapter.AddDataItem($"axis_{i}_pos", axis.MachinePosition, ts);
            //}

            //asset.CuttingToolLifeCycle.ToolLife.Value++;
            //foreach (var asset in _dataSource.ToolAssets)
            //{
            //    Adapter.AddAsset(asset);
            //}

            // The Adapter (ShdrAdapter) handles sending the data to the MTConnect Agent using the SHDR Protocol
        }
    }
}
