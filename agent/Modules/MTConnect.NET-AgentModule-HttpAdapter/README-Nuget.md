![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect HTTP Adapter Agent Module
This Agent Module implements an adapter to read from other MTConnect Agents using the MTConnect REST Protocol

## Configuration
```yaml
- http-adapter:
    address: localhost
    port: 5000
    deviceKey: M12346
    interval: 100
```

* `address` - The client Agent IP Address or Hostname to read from.

* `port` - The port number to read from.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
