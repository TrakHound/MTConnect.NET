# MTConnect.NET-SysML
Classes to handle the read and process the [MTConnect SysML Model](https://model.mtconnect.org/)

## Download
<table>
    <tbody>
        <tr>
            <td>Nuget</td>
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
