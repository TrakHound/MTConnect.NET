![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-JSON-cppagent
MTConnect.NET-JSON is an extension library to MTConnect.NET that provides reading and writing as the JSON format supported by the MTConnect C++ Agent (json version 2.0)

>**Note:** This Module is still under development and may be deprecated in the future. Please feel free to leave feedback or create Issues on GitHub.

## Overview
This library is used to Read and Write using the JSON format from an MTConnect C++ Agent using its Json version 2.0. This can be used for clients and servers.

## Document Format ID (REST and latest version of MQTT)
```
JSON-CPPAGENT
```

## Document Format ID (older versions of MQTT)
```
JSON-CPPAGENT-MQTT
```

The above document format IDs can be used to specify a ResponseDocumentFormatter or EntityFormatter to output using this library
