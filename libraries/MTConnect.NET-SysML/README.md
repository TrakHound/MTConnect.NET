![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect.NET-SysML
Classes to handle the read and process the [MTConnect SysML Model](https://model.mtconnect.org/)

## Nuget
<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Package Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET-SysML</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-SysML?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-SysML">https://www.nuget.org/packages/MTConnect.NET-SysML</a></td>
        </tr>
    </tbody>
</table>

## Overview
Based on the [MTConnectTranspiler](https://github.com/mtconnect/MtconnectTranspiler) project to parse the SysML file and generate source files

## Usage
```c#
using MTConnect.SysML;

// Parse the SysML file and create a model object
var mtconnectModel = MTConnectModel.Parse(@"C:\Users\MTConnect\Downloads\MTConnectSysMLModel.xml");
```
