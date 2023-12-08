![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect MQTT Adapter Agent Module
This Agent Module implements an adapter to read from an MQTT broker

## Configuration
```yaml
- mqtt-adapter:
    server: localhost
    port: 1883
    topic: cnc-01/input
    deviceKey: M12346
```

* `server` - The MQTT broker hostname.

* `port` - The MQTT broker port number.

* `topic` - The MQTT topic to subscribe to

* `deviceKey` - The UUID or Name of the Device to read data for.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
