![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-JSON-cppagent
MTConnect.NET-JSON-cppagent is an extension library to MTConnect.NET that provides reading and writing as the JSON format supported by the MTConnect C++ Agent (json version 2.0)

>**Note:** This Module is still under development and may be deprecated in the future. Please feel free to leave feedback or create Issues on GitHub.

## Overview
This library is used to Read and Write using the JSON format from an MTConnect C++ Agent using its Json version 2.0. This can be used for clients and servers.

## Document Format ID (REST and MQTT)
```
JSON-cppagent
```

## Document Format ID (MQTT, one entity per payload)
```
JSON-cppagent-mqtt
```
This ID uses the same JSON format, but an MTConnectDevices document is reduced to its first Device and an MTConnectAssets document is reduced to a single Asset so that each MQTT topic carries one entity. This is the default Document Format ID of the MTConnectMqttClient class.

The above document format IDs (not case sensitive) can be used to specify a ResponseDocumentFormatter or EntityFormatter to output using this library. The EntityFormatter formats Devices and Assets. It does not format Observations.
