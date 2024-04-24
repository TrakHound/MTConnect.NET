# Standalone MTConnect Agent
> https://github.com/TrakHound/MTConnect.NET/tree/version-6.0/agent/MTConnect.NET-Agent

# Embedded MTConnect Agent
An MTConnect Agent can be embedded into an application where the DataSource(s) can be read and the MTConnect Agent can be combined into the same application. This eliminates the need to transfer data from an Adapter to an Agent (typically using the SHDR protocol).

## Option 1
The first option is to use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to install the MTConnect.NET-Agent-Template using the below commands:

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

