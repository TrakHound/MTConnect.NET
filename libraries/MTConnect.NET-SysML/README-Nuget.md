![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-SysML
Classes to handle the read and process the [MTConnect SysML Model](https://model.mtconnect.org/)

## Overview
Based on the [MTConnectTranspiler](https://github.com/mtconnect/MtconnectTranspiler) project to parse the SysML file and generate source files

## Usage
```c#
using MTConnect.SysML;

// Parse the SysML file and create a model object
var mtconnectModel = MTConnectModel.Parse(@"C:\Users\MTConnect\Downloads\MTConnectSysMLModel.xml");
```
