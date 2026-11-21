![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect HTTP Adapter Agent Module
This Agent Module implements an adapter to read from other MTConnect Agents using the MTConnect REST Protocol

## Configuration
```yaml
modules:
  - http-adapter:
      address: localhost
      port: 5000
      deviceKey: M12346
      interval: 100
      heartbeat: 10000
      useSSL: false
      currentOnly: false
      useStreaming: true
```

* `address` - The client Agent IP Address or Hostname to read from. This is required; the module does not connect if it is not set.

* `port` - The port number to read from. Default is `5000`.

* `deviceKey` - A key used to identify the connection. It is used to create the ID of the Adapter Component that is added to the Agent Device and is appended to the Adapter URI. It does not select the Device to read: every Device of the client Agent is read unless `devices` is set.

* `interval` - The Interval (in milliseconds) to receive new data at. Default is `500`.

* `heartbeat` - The Heartbeat (in milliseconds) to use to maintain a connection to the Agent. Default is `1000`.

* `useSSL` - Set to 'true' if using HTTPS. Default is `false`.

* `currentOnly` - Gets or Sets whether the stream requests a Current (true) or a Sample (false). Default is `false`.

* `useStreaming` - Gets or Sets whether the client should use Streaming (true) or Polling (false). Default is `true`.

* `outputConnectionInformation` - Sets whether the Adapter URI DataItem is added to the Adapter Component on the Agent Device. Default is `true`.

* `devices` - Sets the Devices to read from the client Agent and how they are mapped to Devices in this Agent. Each key is the Name or UUID of a Device to read from the client Agent. If not set, every Device of the client Agent is read and added using its own UUID.
    * `uuid` - The UUID of the Device in this Agent. Observations and Assets that are read are added to the Device with this UUID
    * `name` - The Name of the Device in this Agent. Used to add Observations and Assets if `uuid` is not set
    * `id` - The ID of the Device in this Agent

  If this Agent does not already have a Device that matches the key, the Device is added from the client Agent's Probe response with its `uuid`, `id`, and `name` replaced by the values that are set.

### Example
Read the Device named "VMC-3Axis" from the Agent at "192.168.1.50" and add it to this Agent as the Device with the UUID "M12346"
```yaml
modules:
  - http-adapter:
      address: 192.168.1.50
      port: 5000
      devices:
        VMC-3Axis:
          uuid: M12346
          name: M12346
```


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
