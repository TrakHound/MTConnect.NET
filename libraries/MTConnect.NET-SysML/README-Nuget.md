![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-SysML
Classes to read and process the [MTConnect SysML Model](https://model.mtconnect.org/). Supports parsing the v2.7 XMI; cross-package parent resolver via `MTConnectClassModel.ResolveDanglingParents`.

This library targets .NET 6.0, 7.0, 8.0, 9.0, and 10.0 only (it does not target .NET Framework or .NET Standard like the other MTConnect.NET libraries).

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
