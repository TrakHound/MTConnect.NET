![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-Services
Classes to run Agent and Adapter applications as a Windows Service

## Overview
This library contains two abstract classes that derive from `System.ServiceProcess.ServiceBase`:
- [MTConnectAgentService](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Services/MTConnectAgentService.cs) : Override `StartAgent(string configurationPath)` and `StopAgent()` to start and stop an Agent when the Windows Service is started and stopped
- [MTConnectAdapterService](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Services/MTConnectAdapterService.cs) : Override `StartAdapter(string configurationPath)` and `StopAdapter()` to start and stop an Adapter when the Windows Service is started and stopped

Both classes also contain the `InstallService()`, `RemoveService()`, `StartService()`, and `StopService()` methods used to manage the Windows Service, and the `OnLogInformation()`, `OnLogWarning()`, and `OnLogError()` methods that can be overridden to handle log messages. The service methods only run on Windows.

## Usage
```c#
using MTConnect.Services;

public class AgentService : MTConnectAgentService
{
    protected override void StartAgent(string configurationPath)
    {
        // Start the Agent
    }

    protected override void StopAgent()
    {
        // Stop the Agent
    }
}
```

```c#
var service = new AgentService();

// Install and Start the Windows Service
service.InstallService();
service.StartService();

// Run as a Windows Service (when the application is started by the Service Control Manager)
System.ServiceProcess.ServiceBase.Run(service);
```
