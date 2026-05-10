![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

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

## Code generation

This library is the parsing layer. The C# code-generation tool that consumes
the parsed model and emits the partial-class `.g.cs` files under
`libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-JSON-cppagent/`,
and `libraries/MTConnect.NET-XML/` lives in
[`build/MTConnect.NET-SysML-Import/`](https://github.com/TrakHound/MTConnect.NET/tree/master/build/MTConnect.NET-SysML-Import).
See its `README.md` for how to regenerate the model when a new MTConnect
Standard version is released, including the cross-platform CLI, the
cross-package parent resolver, and the determinism guarantee (a regen
against a pinned XMI tag must produce zero diff).
