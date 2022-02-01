# SHDR Adapters
Adapter classes to handle the SHDR Agent Adapter Protocol associated with the MTConnect Standard.

## Overview
The [ShdrAdapter](ShdrAdapter.cs) class handles the TCP connection to the Agent. SHDR conversion is handled in each individual class:
- [ShdrDataItem](ShdrDataItem.cs) : Handles converting Events and/or Samples with a Representation of VALUE to the appropriate SHDR format.
- [ShdrCondition](ShdrCondition.cs) : Handles converting Conditions to the appropriate SHDR format 
- [ShdrTimeSeries](ShdrTimeSeries.cs) : Handles converting Samples with a Representation of TIME_SERIES to the appropriate SHDR format
- [ShdrDataSet](ShdrDataSet.cs) : Handles converting Events and/or Samples with a Representation of DATA_SET to the appropriate SHDR format
- [ShdrTable](ShdrTable.cs) : Handles converting Events and/or Samples with a Representation of TABLE to the appropriate SHDR format

The [ShdrAdapterClient](ShdrAdapterClient.cs) class handles the TCP connection to read from the Adapter and add data to an IMTConnectAgent class.

## Usage
There are several different ways to setup and add data to the ShdrAdapter

### Initialization
```c#
using MTConnect.Adapters.Shdr;

string deviceName = "OKUMA.Lathe";
ShdrAdpater adapter = new ShdrAdapter(deviceName);
adapter.Start();
```

### Add DataItems Individually
```c#
// DataItemId and CDATA
adapter.AddDataItem(new ShdrDataItem("L2estop", "ARMED"));

// DataItemId, CDATA, and Timestamp
adapter.AddDataItem(new ShdrDataItem("L2estop", "ARMED", DateTime.UtcNow));
```
> If no timestamp is given then the timestamp will be set when the DataItem is sent to the Agent. To insure that all dataitems have the same timestamp, it can be set explicitly.

### Add List of DataItems
```c#
var dataItems = new List<ShdrDataItem>();

dataItems.Add(new ShdrDataItem("L2p1execution", "READY"));
dataItems.Add(new ShdrDataItem("L2p1Fovr", 100));
dataItems.Add(new ShdrDataItem("L2p1partcount", 15));
dataItems.Add(new ShdrDataItem("L2p1Fact", 250));

adapter.AddDataItems(dataItems);
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

### Use Models
The DeviceModel class can be used to add data to an ShdrAdapter by setting the properties of the DeviceModel and adding those Observations to the Adapter as shown below:
```c#
using MTConnect.Models;
using MTConnect.Streams.Events;

DeviceModel deviceModel = new DeviceModel("OKUMA.Lathe");
deviceModel.Controller.EmergencyStop = EmergencyStop.ARMED;

adapter.AddDataItems(deviceModel.GetObservations());
```

## Conditions
```c#
ShdrCondition condition = new ShdrCondition("L2p1system", ConditionLevel.FAULT);
condition.NativeCode = "404";
condition.NativeSeverity = "100";
condition.Qualifier = "LOW";
condition.Message = "Testing from new adapter";

adapter.AddCondition(condition);
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

## DataSets
```c#
List<DataSetEntry> dataSetEntries = new List<DataSetEntry>();
dataSetEntries.Add(new DataSetEntry("V1", 5));
dataSetEntries.Add(new DataSetEntry("V2", 205));

ShdrDataSet dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries);

adapter.AddDataSet(dataSet);
```

## Tables
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

ShdrTable table = new ShdrTable("L2p1ToolTable", tableEntries);

adapter.AddTable(table);
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

### ShdrTable.ToSTring()
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
