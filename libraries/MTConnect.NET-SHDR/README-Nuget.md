![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-SHDR
Classes to handle the SHDR Agent Adapter Protocol associated with the MTConnect Standard.

## Overview
The ShdrAdapter classes handle the TCP connection to the Agent:

- [ShdrAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrAdapter.cs) : Sends the most recent values On-Demand using the SendChanged() method. This is used when full control of the communication is needed.
- [ShdrIntervalAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalAdapter.cs) : Sends the most recent values at the specified Interval. This is used when a set interval is adequate and the most recent value is all that is needed
- [ShdrQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrQueueAdapter.cs) : Queues all values that are sent from the PLC and sends them all on demand using the SendBuffer() method. This is used when all values are needed and full control of the communication is needed.
- [ShdrIntervalQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalQueueAdapter.cs) : Queues all values that are sent from the PLC and sends any queued values at the specified Interval. This is used when all values are needed but an interval is adequate.

SHDR conversion is handled in each individual class:
- [ShdrDataItem](Shdr/ShdrDataItem.cs) : Handles converting Events and/or Samples with a Representation of VALUE to the appropriate SHDR format.
- [ShdrCondition](Shdr/ShdrCondition.cs) : Handles converting Conditions to the appropriate SHDR format 
- [ShdrTimeSeries](Shdr/ShdrTimeSeries.cs) : Handles converting Samples with a Representation of TIME_SERIES to the appropriate SHDR format
- [ShdrDataSet](Shdr/ShdrDataSet.cs) : Handles converting Events and/or Samples with a Representation of DATA_SET to the appropriate SHDR format
- [ShdrTable](Shdr/ShdrTable.cs) : Handles converting Events and/or Samples with a Representation of TABLE to the appropriate SHDR format
- [ShdrAsset](Shdr/ShdrAsset.cs) : Handles converting Assets to the appropriate SHDR format

The [ShdrAdapterClient](Adapters/Shdr/ShdrAdapterClient.cs) class handles the TCP connection to read from the Adapter and add data to an IMTConnectAgent class.

## Usage
There are several different ways to setup and add data to the ShdrAdapter

### ShdrAdapter
The example below creates a new ShdrAdapter.
The added DataItem will be sent using the "adapter.SendChanged()" method
```c#
using MTConnect.Adapters.Shdr;

ShdrAdapter adapter = new ShdrAdapter();
adapter.Start();

adapter.AddDataItem("L2estop", "ARMED");
adapter.SendChanged();
```
#### SHDR Output
```
2023-01-26T16:48:17.0206852Z|L2estop|ARMED
```

### ShdrAdapter (specific Port)
The example below creates a new ShdrAdapter on the 7980 TCP Port.
```c#
using MTConnect.Adapters.Shdr;

ShdrAdapter adapter = new ShdrAdapter(7980);
```

### ShdrAdapter (specific Device)
The example below creates a new ShdrAdapter for the "OKUMA-Lathe" device.
The added DataItem will be sent using the "adapter.SendChanged()" method
```c#
using MTConnect.Adapters.Shdr;

ShdrAdapter adapter = new ShdrAdapter("OKUMA-Lathe", 7980);
adapter.Start();

adapter.AddDataItem("L2estop", "ARMED");
adapter.SendChanged();
```
#### SHDR Output
```
2023-01-26T20:54:34.1694626Z|OKUMA-Lathe:L2estop|ARMED
```

### ShdrAdapter (specific Device and Port)
The example below creates a new ShdrAdapter for the "OKUMA-Lathe" device on the 7980 TCP Port.
```c#
using MTConnect.Adapters.Shdr;

ShdrAdapter adapter = new ShdrAdapter("OKUMA-Lathe", 7980);
```

### ShdrIntervalAdapter
The example below creates a new ShdrIntervalAdapter and sets the Interval to 500ms.
Only the most recent DataItem with the ID = "L2estop" will be sent after 500ms has elapsed.
```c#
using MTConnect.Adapters.Shdr;

ShdrIntervalAdapter adapter = new ShdrIntervalAdapter(interval: 500);
adapter.Start();

adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
```
#### SHDR Output
```
2023-01-26T17:22:10.3128693Z|L2estop|TRIGGERED
```

### ShdrQueueAdapter
The example below creates a new ShdrQueueAdapter.
All of the added DataItems will be sent using the "adapter.SendBuffer()" method
```c#
using MTConnect.Adapters.Shdr;

ShdrQueueAdapter adapter = new ShdrQueueAdapter();
adapter.Start();

adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
adapter.SendBuffer();
```
#### SHDR Output
```
2023-01-26T17:22:10.2882160Z|L2estop|ARMED
2023-01-26T17:22:10.3125127Z|L2estop|TRIGGERED
2023-01-26T17:22:10.3128389Z|L2estop|ARMED
2023-01-26T17:22:10.3128693Z|L2estop|TRIGGERED
```

### ShdrIntervalQueueAdapter
The example below creates a new ShdrIntervalQueueAdapter and sets the Interval to 500ms.
All of the added DataItems will be sent after 500ms has elapsed.
```c#
using MTConnect.Adapters.Shdr;

ShdrIntervalQueueAdapter adapter = new ShdrIntervalQueueAdapter(interval: 500);
adapter.Start();

adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
adapter.AddDataItem("L2estop", "ARMED");
adapter.AddDataItem("L2estop", "TRIGGERED");
```
#### SHDR Output
```
2023-01-26T17:22:10.2882160Z|L2estop|ARMED
2023-01-26T17:22:10.3125127Z|L2estop|TRIGGERED
2023-01-26T17:22:10.3128389Z|L2estop|ARMED
2023-01-26T17:22:10.3128693Z|L2estop|TRIGGERED
```

## Configuration

* `Id` - Get a unique identifier for the Adapter

* `DeviceKey` - The Name or UUID of the Device to create a connection for

* `Port` - The TCP Port used for communication

* `Heartbeat` - The heartbeat used to maintain a connection between the Adapter and the Agent

* `ConnectionTimeout` - The amount of time (in milliseconds) to allow for a connection attempt to the Agent
 
* `ReconnectInterval` - The amount of time (in milliseconds) between adapter reconnection attempts

* `MultilineAssets` - Use multiline Assets

* `MultilineDevices` - Sets the default for Converting Units when adding Observations

* `FilterDuplicates` - Determines whether to filter out duplicate data

* `OutputTimestamps` - Determines whether to output Timestamps for each SHDR line

### OutputTimestamps
Timestamps may be output in the SHDR protocol in order to pass the timestamp from the Adapter instead of allowing the Agent to apply a timestamp.

**OutputTimestamp** = true
```
2023-01-27T03:47:53.4477372Z|L2estop|ARMED
```
**OutputTimestamp** = false
```
L2estop|ARMED
```

## Sending Data

### Add DataItems Individually
```c#
// DataItemId and CDATA
adapter.AddDataItem("L2estop", "ARMED");

// DataItemId and CDATA, and Timestamp
adapter.AddDataItem("L2estop", "ARMED", DateTime.UtcNow);

// DataItemId and CDATA as ShdrDataItem
adapter.AddDataItem(new ShdrDataItem("L2estop", "ARMED"));

// DataItemId, CDATA, and Timestamp as ShdrDataItem
adapter.AddDataItem(new ShdrDataItem("L2estop", "ARMED", DateTime.UtcNow));
```
> If no timestamp is given then the timestamp will be set when the DataItem is sent to the Agent. To insure that all dataitems have the same timestamp, it can be set explicitly.

### Add List of DataItems
```c#
var ts = DateTime.UtcNow;
var dataItems = new List<ShdrDataItem>();

dataItems.Add(new ShdrDataItem("L2p1execution", "READY", ts));
dataItems.Add(new ShdrDataItem("L2p1Fovr", 100, ts));
dataItems.Add(new ShdrDataItem("L2p1partcount", 15, ts));
dataItems.Add(new ShdrDataItem("L2p1Fact", 250, ts));

adapter.AddDataItems(dataItems);
```
#### Output
```
2023-01-26T20:50:53.6161001Z|L2p1execution|READY|L2p1Fovr|100|L2p1partcount|15|L2p1Fact|250
```

### Store DataItems in Variables
```c#
ShdrDataItem executionDataItem = new ShdrDataItem("L2p1execution");
ShdrDataItem feedrateOverrideDataItem = new ShdrDataItem("L2p1Fovr");
ShdrDataItem partCountDataItem = new ShdrDataItem("L2p1partcount");
ShdrDataItem feedrateActualDataItem = new ShdrDataItem("L2p1Fact");

executionDataItem.Value = "READY";
feedrateOverrideDataItem.Value = 100;
partCountDataItem.Value = 15;
feedrateActualDataItem.Value = 250;

adapter.AddDataItem(executionDataItem);
adapter.AddDataItem(feedrateOverrideDataItem);
adapter.AddDataItem(partCountDataItem);
adapter.AddDataItem(feedrateActualDataItem);
```

### Send DataItem Manually
DataItems can be added to the Adapter and immediately send to the Agent using the "SendDataItem()" method
```c#
adapter.SendDataItem("L2estop", "ARMED");
```

## Conditions
The ShdrCondition class is used to send MTConnect Condition data using the 5 primary values Level, NativeCode, NativeSeverity, Qualifier, and Message (referred to as Result in the MTConnect Standard). 
A Condition is made up of 1 or more FaultStates. Each FaultState is represented by the ShdrFaultState class.

#### Add a Fault Condition
```c#
ShdrCondition condition = new ShdrCondition("L2p1system", ConditionLevel.FAULT);
adapter.AddCondition(condition);
```

A Condition can also be added by using the built-in functions:

#### Set the Condition to Normal
```c#
ShdrCondition condition = new ShdrCondition("L2p1system");
condition.Normal();

adapter.AddCondition(condition);
```

#### Set the Condition to Warning
```c#
ShdrCondition condition = new ShdrCondition("L2p1system");
condition.Warning("Not Found", "404", "100", ConditionQualifier.LOW);

adapter.AddCondition(condition);
```

#### Set the Condition to Fault
```c#
ShdrCondition condition = new ShdrCondition("L2p1system");
condition.Fault("Internal Error", "500", "10254", ConditionQualifier.HIGH);

adapter.AddCondition(condition);
```

#### Set multiple FaultStates
```c#
ShdrCondition condition = new ShdrCondition("L2p1coolant");
condition.AddWarning("Coolant Level Low", "47321", qualifier: ConditionQualifier.LOW);
condition.AddWarning("Coolant Temperature High", "98712", qualifier: ConditionQualifier.HIGH);

adapter.AddCondition(condition);
```

#### Set the Condition to Unavailable
```c#
ShdrCondition condition = new ShdrCondition("L2p1system");
condition.Unavailable();

adapter.AddCondition(condition);
```

#### Add FaultStates Individually
```c#
ShdrCondition condition = new ShdrCondition("L2p1system");

ShdrFaultState faultState = new ShdrFaultState();
faultState.NativeCode = "404";
faultState.NativeSeverity = "100";
faultState.Qualifier = "LOW";
faultState.Message = "Testing from new adapter";

condition.AddFaultState(faultState);
adapter.AddCondition(condition);
```

### Send Condition Manually
Conditions can be added to the Adapter and immediately send to the Agent using the "SendCondition()" method
```c#
adapter.SendCondition(condition);
```

## TimeSeries
```c#
List<double> samples = new List<double>();
samples.Add(12);
samples.Add(15);
samples.Add(14);
samples.Add(18);
samples.Add(25);
samples.Add(30);

ShdrTimeSeries timeSeries = new ShdrTimeSeries("L2p1Sensor", samples, 100);

adapter.AddTimeSeries(timeSeries);
```
### Output
```
2023-01-26T20:39:28.1540686Z|L2p1Sensor|6|100|12 15 14 18 25 30
```

### Send TimeSeries Manually
TimeSeries can be added to the Adapter and immediately send to the Agent using the "SendTimeSeries()" method
```c#
adapter.SendTimeSeries(timeSeries);
```

## DataSets
```c#
List<DataSetEntry> dataSetEntries = new List<DataSetEntry>();
dataSetEntries.Add(new DataSetEntry("V1", 5));
dataSetEntries.Add(new DataSetEntry("V2", 205));

ShdrDataSet dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries);

adapter.AddDataSet(dataSet);
```
### Output
```
2023-01-26T20:40:30.6718334Z|L2p1Variables|V1=5 V2=205
```

### Send DataSets Manually
DataSets can be added to the Adapter and immediately send to the Agent using the "SendDataSets()" method
```c#
adapter.SendDataSets(dataSet);
```

## Tables
```c#
List<TableEntry> tableEntries = new List<TableEntry>();

// Tool 1
List<TableCell> t1Cells = new List<TableCell>();
t1Cells.Add(new TableCell("LENGTH", 7.123));
t1Cells.Add(new TableCell("DIAMETER", 0.494));
t1Cells.Add(new TableCell("REMAINING_LIFE", 35));
tableEntries.Add(new TableEntry("T1", t1Cells));

// Tool 2
List<TableCell> t2Cells = new List<TableCell>();
t2Cells.Add(new TableCell("LENGTH", 10.456));
t2Cells.Add(new TableCell("DIAMETER", 0.125));
t2Cells.Add(new TableCell("REMAINING_LIFE", 100));
tableEntries.Add(new TableEntry("T2", t2Cells));

// Tool 3
List<TableCell> t3Cells = new List<TableCell>();
t3Cells.Add(new TableCell("LENGTH", 6.251));
t3Cells.Add(new TableCell("DIAMETER", 1.249));
t3Cells.Add(new TableCell("REMAINING_LIFE", 93));
tableEntries.Add(new TableEntry("T3", t3Cells));

ShdrTable table = new ShdrTable("L2p1ToolTable", tableEntries);

adapter.AddTable(table);
```
### Output
```
2023-01-26T20:40:55.8702675Z|L2p1ToolTable|T1={LENGTH=7.123 DIAMETER=0.494 TOOL_LIFE=0.35} T2={LENGTH=10.456 DIAMETER=0.125 TOOL_LIFE=1} T3={LENGTH=6.251 DIAMETER=1.249 TOOL_LIFE=0.93}
```

### Send Tables Manually
Tables can be added to the Adapter and immediately send to the Agent using the "SendTables()" method
```c#
adapter.SendTables(table);
```

## Assets
MTConnect Assets are sent by first defining the Asset using the appropriate class (CuttingToolAsset, FileAsset, etc.) then using the "AddAsset()".

### CuttingTool Asset
```c#
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;

var tool = new CuttingToolAsset();
tool.AssetId = "5.12";
tool.ToolId = "12";
tool.CuttingToolLifeCycle = new CuttingToolLifeCycle
{
    Location = new Location { Type = LocationType.SPINDLE },
    ProgramToolNumber = "12",
    ProgramToolGroup = "5"
};
tool.CuttingToolLifeCycle.Measurements.Add(new FunctionalLengthMeasurement(7.6543));
tool.CuttingToolLifeCycle.Measurements.Add(new CuttingDiameterMaxMeasurement(0.375));
tool.CuttingToolLifeCycle.CuttingItems.Add(new CuttingItem
{
    ItemId = "12.1",
    Locus = CuttingItemLocas.FLUTE.ToString()
});
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.AVAILABLE);
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.NEW);
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.MEASURED);
tool.DateTime = DateTime.Now;

adapter.AddAsset(tool);
```
#### Output (with MultilineAssets = true)
```
2023-01-26T17:56:59.9694353Z|@ASSET@|5.12|CuttingTool|--multiline--W5XZBJ2QZV
<CuttingTool assetId="5.12" timestamp="2023-01-26T12:56:59.4778578-05:00" toolId="12">
  <CuttingToolLifeCycle>
    <CutterStatus>
      <Status>AVAILABLE</Status>
      <Status>NEW</Status>
      <Status>MEASURED</Status>
    </CutterStatus>
    <Location type="SPINDLE">0</Location>
    <ProgramToolGroup>5</ProgramToolGroup>
    <ProgramToolNumber>12</ProgramToolNumber>
    <Measurements>
      <FunctionalLength units="MILLIMETER" code="LF">7.6543</FunctionalLength>
      <CuttingDiameterMax units="MILLIMETER" code="DC">0.375</CuttingDiameterMax>
    </Measurements>
    <CuttingItems count="1">
      <CuttingItem itemId="12.1">
        <Locus>FLUTE</Locus>
      </CuttingItem>
    </CuttingItems>
  </CuttingToolLifeCycle>
</CuttingTool>
--multiline--W5XZBJ2QZV
```

### File Asset
```c#
using MTConnect.Assets.Files;

var file = new FileAsset();
file.DateTime = DateTime.UtcNow;
file.AssetId = "file.test";
file.Size = 12346;
file.VersionId = "test-v1";
file.State = FileState.PRODUCTION;
file.Name = "file-123.txt";
file.MediaType = "text/plain";
file.ApplicationCategory = ApplicationCategory.DEVICE;
file.ApplicationType = ApplicationType.DATA;
file.FileLocation = new FileLocation(@"C:\temp\file-123.txt");
file.CreationTime = DateTime.Now;

adapter.AddAsset(file);
```
#### Output (with MultilineAssets = true)
```
2023-01-26T18:01:50.3085245Z|@ASSET@|file.test|File|--multiline--6UH71Y7IYW
<File assetId="file.test" timestamp="2023-01-26T18:01:49.9929867Z" name="file-123.txt" mediaType="text/plain" applicationCategory="DEVICE" applicationType="DATA" size="12346" versionId="test-v1" state="PRODUCTION">
  <FileLocation href="C:\temp\file-123.txt" />
  <CreationTime>2023-01-26T13:01:49.9939538-05:00</CreationTime>
</File>
--multiline--6UH71Y7IYW
```

### Add Multiple Assets
```c#
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;
using MTConnect.Assets.Files;

var assets = new List<IAsset>();


// Add the Cutting Tool Asset to the "assets" list variable
var tool = new CuttingToolAsset();
tool.AssetId = "5.12";
tool.ToolId = "12";
tool.CuttingToolLifeCycle = new CuttingToolLifeCycle
{
    Location = new Location { Type = LocationType.SPINDLE },
    ProgramToolNumber = "12",
    ProgramToolGroup = "5"
};
tool.CuttingToolLifeCycle.Measurements.Add(new FunctionalLengthMeasurement(7.6543));
tool.CuttingToolLifeCycle.Measurements.Add(new CuttingDiameterMaxMeasurement(0.375));
tool.CuttingToolLifeCycle.CuttingItems.Add(new CuttingItem
{
    ItemId = "12.1",
    Locus = CuttingItemLocas.FLUTE.ToString()
});
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.AVAILABLE);
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.NEW);
tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.MEASURED);
tool.DateTime = DateTime.Now;

assets.Add(tool);


// Add the File Asset to the "assets" list variable
var file = new FileAsset();
file.DateTime = DateTime.UtcNow;
file.AssetId = "file.test";
file.Size = 12346;
file.VersionId = "test-v1";
file.State = FileState.PRODUCTION;
file.Name = "file-123.txt";
file.MediaType = "text/plain";
file.ApplicationCategory = ApplicationCategory.DEVICE;
file.ApplicationType = ApplicationType.DATA;
file.FileLocation = new FileLocation(@"C:\temp\file-123.txt");
file.CreationTime = DateTime.Now;

assets.Add(file);

adapter.AddAssets(assets);
```

### Send Asset Manually
Assets can be added to the Adapter and immediately send to the Agent using the "SendAsset()" method
```c#
using MTConnect.Assets.Files;

var file = new FileAsset();
// ..
// ..
// ..

adapter.SendAsset(file);
```

### Remove Individual Asset by AssetId
```c#
adapter.RemoveAsset("file.test");
```
#### Output
```
2023-01-26T18:21:57.8208518Z|@REMOVE_ASSET@|file.test
```

### Remove All Assets of a specified Type
```c#
adapter.RemoveAllAssets("File");
```
#### Output
```
2023-01-26T18:31:23.6664032Z|@REMOVE_ALL_ASSETS@|File
```

## SHDR Conversion
Conversion to and from an SHDR message is done through methods in the ShdrDataItem, ShdrCondition, ShdrTimeSeries, ShdrDataSet, and ShdrTable classes. Each class overrides the base "ToString()" method and also contains a "FromString()" method. The "ToString()" method creates an SHDR compatible string from the object and the "FromString()" creates an object from an SHDR string.

### ShdrDataItem.ToString()
```c#
ShdrDataItem availableDataItem = new ShdrDataItem("L2avail", Availability.AVAILABLE, DateTime.UtcNow);
Console.WriteLine(availableDataItem);
```
> 2022-02-01T13:53:03.6940000Z|L2avail|AVAILABLE

### ShdrCondition.ToString()
```c#
ShdrCondition condition = new ShdrCondition("L2p1system", ConditionLevel.FAULT, DateTime.UtcNow);
condition.NativeCode = "404";
condition.NativeSeverity = "100";
condition.Qualifier = "LOW";
condition.Message = "Testing from new adapter";
Console.WriteLine(condition);
```
> 2022-02-01T13:55:11.8460000Z|L2p1system|FAULT|404|100|LOW|Testing from new adapter

### ShdrTimeSeries.ToString()
```c#
List<double> samples = new List<double>();
samples.Add(12);
samples.Add(15);
samples.Add(14);
samples.Add(18);
samples.Add(25);
samples.Add(30);

ShdrTimeSeries timeSeries = new ShdrTimeSeries("L2p1Sensor", samples, 100, DateTime.UtcNow);
Console.WriteLine(timeSeries);
```
> 2022-02-01T13:56:58.7700000Z|L2p1Sensor|6|100|12 15 14 18 25 30

### ShdrDataSet.ToString()
```c#
List<DataSetEntry> dataSetEntries = new List<DataSetEntry>();
dataSetEntries.Add(new DataSetEntry("V1", 5));
dataSetEntries.Add(new DataSetEntry("V2", 205));

ShdrDataSet dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries, DateTime.UtcNow);
Console.WriteLine(dataSet);
```
> 2022-02-01T13:58:31.8150000Z|L2p1Variables|V1=5 V2=205

### ShdrTable.ToString()
```c#
List<TableEntry> tableEntries = new List<TableEntry>();

// Tool 1
List<TableCell> t1Cells = new List<TableCell>();
t1Cells.Add(new TableCell("LENGTH", 7.123));
t1Cells.Add(new TableCell("DIAMETER", 0.494));
t1Cells.Add(new TableCell("TOOL_LIFE", 0.35));
tableEntries.Add(new TableEntry("T1", t1Cells));

// Tool 2
List<TableCell> t2Cells = new List<TableCell>();
t2Cells.Add(new TableCell("LENGTH", 10.456));
t2Cells.Add(new TableCell("DIAMETER", 0.125));
t2Cells.Add(new TableCell("TOOL_LIFE", 1));
tableEntries.Add(new TableEntry("T2", t2Cells));

// Tool 3
List<TableCell> t3Cells = new List<TableCell>();
t3Cells.Add(new TableCell("LENGTH", 6.251));
t3Cells.Add(new TableCell("DIAMETER", 1.249));
t3Cells.Add(new TableCell("TOOL_LIFE", 0.93));
tableEntries.Add(new TableEntry("T3", t3Cells));

ShdrTable table = new ShdrTable("L2p1ToolTable", tableEntries, DateTime.UtcNow);
Console.WriteLine(table);
```
> 2022-02-01T13:59:47.5980000Z|L2p1ToolTable|T1={LENGTH=7.123 DIAMETER=0.494 TOOL_LIFE=0.35} T2={LENGTH=10.456 DIAMETER=0.125 TOOL_LIFE=1} T3={LENGTH=6.251 DIAMETER=1.249 TOOL_LIFE=0.93}
