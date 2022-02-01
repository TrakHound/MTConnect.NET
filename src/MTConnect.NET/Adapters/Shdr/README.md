# SHDR Adapters
Adapter classes to handle the SHDR Agent Adapter Protocol associated with the MTConnect Standard.

## Overview
The [ShdrAdapter](ShdrAdapter.cs) class handles the TCP connection to the Agent. SHDR conversion is handled in each individual class:
- [ShdrDataItem](ShdrDataItem.cs) : Handles converting Events and/or Samples with a Representation of VALUE to the appropriate SHDR format.
- [ShdrCondition](ShdrCondition.cs) : Handles converting Conditions to the appropriate SHDR format 
- [ShdrTimeSeries](ShdrTimeSeries.cs) : Handles converting Samples with a Representation of TIME_SERIES to the appropriate SHDR format
- [ShdrDataSet](ShdrDataSet.cs) : Handles converting Events and/or Samples with a Representation of DATA_SET to the appropriate SHDR format
- [ShdrTable](ShdrTable.cs) : Handles converting Events and/or Samples with a Representation of TABLE to the appropriate SHDR format

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
