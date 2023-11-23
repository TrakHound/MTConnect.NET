using MTConnect.Input;
using MTConnect.Observations.Events;
using NLog;

namespace MTConnect.Applications
{
    // This class is used for reading from your DataSource (ex. PLC) and writing to the "Adapter" object that is included in the base class

    // OnRead() : Gets called at the specfied Interval in the Configuration and would be where you can scan the PLC variables that you want to write to the MTConnect Agent

    // OnReadAsync() : Same as OnRead() but is an async method


    internal class PlcDataSource : MTConnectDataSource
    {
        protected readonly Logger _engineLogger = LogManager.GetLogger("engine-logger");
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
            // Using a single Timestamp (per OnRead() call) can consolidate the SHDR output as well as make MTConnect data more "aligned" and easier to process
            var ts = UnixDateTime.Now;
            AddObservation("L2estop", EmergencyStop.ARMED, ts);

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
