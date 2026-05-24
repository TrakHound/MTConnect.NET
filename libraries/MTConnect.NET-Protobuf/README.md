![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-Protobuf
MTConnect.NET-Protobuf is an extension library to MTConnect.NET that is intended to provide reading and writing of MTConnect Response Documents using the Protocol Buffers (Protobuf) binary wire format.

>**Note:** This library is a placeholder and is currently under active development. It does not yet expose a public API. Contributions and feedback are welcome via [GitHub Issues](https://github.com/TrakHound/MTConnect.NET/issues).

For the XML and JSON format libraries that are production-ready today, see [MTConnect.NET-XML](../MTConnect.NET-XML/) and [MTConnect.NET-JSON-cppagent](../MTConnect.NET-JSON-cppagent/).

## Overview
Protocol Buffers is a language-neutral, platform-neutral binary serialization format developed by Google. When complete, this library will provide a compact, high-throughput alternative to the XML and JSON response document formats for scenarios where bandwidth or parse latency is critical—for example, high-frequency MQTT or direct TCP data paths.

## Related Libraries
- [MTConnect.NET-Common](../MTConnect.NET-Common/) — Core MTConnect entity model
- [MTConnect.NET-XML](../MTConnect.NET-XML/) — XML response document format (production-ready)
- [MTConnect.NET-JSON](../MTConnect.NET-JSON/) — JSON response document format
- [MTConnect.NET-JSON-cppagent](../MTConnect.NET-JSON-cppagent/) — JSON (cppagent) response document format (production-ready)

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This library and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
