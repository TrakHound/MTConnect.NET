; MTConnect MQTT Broker Agent Installer
; ------------------------------

#define MyAppName "MTConnect MQTT Broker Agent"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "MTConnect"
#define MyAppURL ""
#define MyAppVerName MyAppName + " " + MyAppVersion

; File Names
#define ExeName "MTConnect-MQTT-Broker-Agent.exe"


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C8E8645E-D497-4064-84A5-D4219390BBD8}

AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppReadmeFile=https://github.com/TrakHound/MTConnect.NET
VersionInfoVersion={#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64

PrivilegesRequired=admin
DisableProgramGroupPage=no
Compression=lzma
SolidCompression=yes

; Names
DisableDirPage=auto
DisableReadyPage=no
DisableFinishedPage=no
OutputDir=bin\Publish
DefaultDirName={autopf}\{#MyAppPublisher}\MTConnect-MQTT-Broker-Agent
UsePreviousAppDir=false
OutputBaseFilename=MTConnect-MQTT-Broker-Agent-Install-v{#MyAppVersion}
UninstallDisplayName={#MyAppName} v{#MyAppVersion}
UninstallFilesDir={app}\Uninstall

[Types]
Name: "win64net7"; Description: "Windows x64 .NET 7"
Name: "win86net7"; Description: "Windows x86 .NET 7"
Name: "win64net6"; Description: "Windows x64 .NET 6"
Name: "win86net6"; Description: "Windows x86 .NET 6"
Name: "win64net48"; Description: "Windows x64 .NET 4.8 Framework"
Name: "win86net48"; Description: "Windows x86 .NET 4.8 Framework"
Name: "win64net461"; Description: "Windows x64 .NET 4.6.1 Framework"
Name: "win86net461"; Description: "Windows x86 .NET 4.6.1 Framework"

[Components]
Name: "win64net7"; Description: "{#MyAppVerName}"; Types: win64net7
Name: "win86net7"; Description: "{#MyAppVerName}"; Types: win86net7
Name: "win64net6"; Description: "{#MyAppVerName}"; Types: win64net6
Name: "win86net6"; Description: "{#MyAppVerName}"; Types: win86net6
Name: "win64net48"; Description: "{#MyAppVerName}"; Types: win64net48
Name: "win86net48"; Description: "{#MyAppVerName}"; Types: win86net48
Name: "win64net461"; Description: "{#MyAppVerName}"; Types: win64net461
Name: "win86net461"; Description: "{#MyAppVerName}"; Types: win86net461
Name: "configuration"; Description: "Configuration Files"; Types: win86net461 win64net461 win86net48 win64net48 win86net6 win64net6 win86net7 win64net7


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{app}"; Permissions: everyone-full
Name: "{app}\logs"; Permissions: everyone-full
Name: "{app}\devices"; Permissions: everyone-full
Name: "{app}\styles"; Permissions: everyone-full

[Files]

; Program Files
Source: "bin\release\net7.0\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net7;
Source: "bin\release\net7.0\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net7;
Source: "bin\release\net6.0\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net6;
Source: "bin\release\net6.0\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net6;
Source: "bin\release\net48\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net48;
Source: "bin\release\net48\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net48;
Source: "bin\release\net461\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net461;
Source: "bin\release\net461\win-x86\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net461;


[Run]

; Install and Run Windows Service
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""stop""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Stopping Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""remove""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Uninstalling Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""install""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Installing Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""start""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Starting Windows Service...";

; Start Browser
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""agent.config.yaml""" ; WorkingDir: {app}; Flags: runhidden postinstall unchecked; Description: Open Agent Configuration File;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""NLog.config""" ; WorkingDir: {app}; Flags: runhidden postinstall unchecked; Description: Open Log Configuration File;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""{app}""" ; Flags: runhidden postinstall unchecked; Description: Open Install Directory;


[UninstallRun]

; Stop Server Service
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""stop""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Stopping Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""remove""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Uninstalling Windows Service...";
