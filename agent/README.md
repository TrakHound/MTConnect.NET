# Standalone MTConnect Agent

<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: left;"></th>
            <th style="text-align: left;">Download Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Installer</td>
            <td><img src="https://img.shields.io/github/downloads/TrakHound/MTConnect.NET/total?style=for-the-badge&logo=github&label=%20&color=%23333"/></td>
            <td><a href="https://github.com/TrakHound/MTConnect.NET/releases/download/v5.4.3/MTConnect-Agent-HTTP-5.4.3-Install.exe">https://github.com/TrakHound/MTConnect.NET/releases</a></td>
        </tr>
        <tr>
            <td>Docker</td>
            <td><img src="https://img.shields.io/docker/pulls/trakhound/mtconnect.net-agent?style=for-the-badge&logo=docker&label=%20&color=%23333"/></td>
            <td><a href="https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent">https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent</a></td>
        </tr>
    </tbody>
</table>

A standalone preconfigured application ready to download is available and supports:
- Modular architecture
    - HTTP Server Module
    - SHDR Adapter Module
    - MQTT Broker Module
    - MQTT Relay Module
    - etc.
- Easy Windows Installer
- Linux Compatible
- Run as a Windows Service
- Docker Image
- Transform input data using Python scripts
- Extensible configuration file and monitors for changes

> [Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/agent/MTConnect.NET-Agent) about the standalone Agent installer

# Embedded MTConnect Agent
An MTConnect Agent can be embedded into an application where the DataSource(s) can be read and the MTConnect Agent can be combined into the same application. This eliminates the need to transfer data from an Adapter to an Agent (typically using the SHDR protocol).

## Option 1
The first option is to use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to install the **[MTConnect.NET-Agent-Template](https://www.nuget.org/packages/MTConnect.NET-Agent-Template)** using the below commands:

### Install Template
```
dotnet new install MTConnect.NET-Agent-Template
```

### Create New Project
This will create a new project using the template in the current working directory
```
dotnet new mtconnect.net-agent
```

## Option 2
The second option is to use the **[MTConnect.NET-Applications-Agents](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents)** Nuget Package directly in your own project. 
```
dotnet add package MTConnect.NET-Applications-Agents
```

