# MTConnect.NET-Agent-Template
An MTConnect Agent can be embedded into an application where the DataSource(s) can be read and the MTConnect Agent can be combined into the same application. This eliminates the need to transfer data from an Adapter to an Agent (typically using the SHDR protocol).

## Setup

#### Install Template
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to install the MTConnect.NET-Agent-Template using the below command:
```
dotnet new install MTConnect.NET-Agent-Template
```

#### Create New Project
This will create a new project using the template in the current working directory
```
dotnet new mtconnect.net-agent
```

## Build & Develop

#### dotnet CLI
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to build the project using the below command:
```
dotnet build -c:Debug
```

#### Run
```
dotnet run -c:Debug
```

## Create Installer
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to build the project using the *Release** configuration using the below command:
```
dotnet build -c:Release
```

#### Install InnoSetup (if not already installed)
information - https://jrsoftware.org/isinfo.php
download - https://jrsoftware.org/isdl.php#stable


