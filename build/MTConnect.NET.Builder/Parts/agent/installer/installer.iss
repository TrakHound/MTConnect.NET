#define MyAppName "[APPLICATION_NAME]"
#define MyAppVersion "[ASSEMBLY_VERSION]"
#define MyAppPublisher "[PUBLISHER]"
#define MyAppURL "http://www.TrakHound.com/"
#define MyAppVerName MyAppName + " " + MyAppVersion

; File Names
#define ExeName "[FILENAME].exe"
#define LicenseFile "[BASE_PATH]\LICENSE.txt"
#define IconFile "trakhound.ico"

;#include "environment.iss"


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{DF165FF0-E42D-417E-AB44-15623C732E81}
;SignTool=TrakHound
SignToolRunMinimized=yes

AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright=Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppReadmeFile=https://github.com/TrakHound/MTConnect.NET
VersionInfoVersion={#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64

PrivilegesRequired=admin
LicenseFile={#LicenseFile}
Compression=lzma
SolidCompression=yes

ChangesEnvironment=true

; Names
OutputDir=[OUTPUT_PATH]
DefaultDirName={autopf}\TrakHound\Instance
UsePreviousAppDir=true
OutputBaseFilename=[OUTPUT_EXE]
UninstallDisplayName={#MyAppName} v{#MyAppVersion}
UninstallFilesDir={app}\Uninstall

; Wizard
WizardStyle=modern
WindowResizable=no
WizardSmallImageFile=[BASE_PATH]\images\installer-top.bmp
DisableDirPage=no
DisableProgramGroupPage=yes
DisableReadyPage=no
DisableFinishedPage=no

; Icon Files
SetupIconFile=[BASE_PATH]\icons\{#IconFile}
UninstallDisplayIcon={app}\{#IconFile}

[CustomMessages]
CreateStartMenuIcon=Create a Start Menu icon
CreateDesktopIcon=Create a Desktop icon

[Types]
Name: "win64net9"; Description: "Windows x64 .NET 9"
Name: "win86net9"; Description: "Windows x86 .NET 9"
Name: "win64net8"; Description: "Windows x64 .NET 8"
Name: "win86net8"; Description: "Windows x86 .NET 8"
Name: "win64net7"; Description: "Windows x64 .NET 7"
Name: "win86net7"; Description: "Windows x86 .NET 7"
Name: "win64net6"; Description: "Windows x64 .NET 6"
Name: "win86net6"; Description: "Windows x86 .NET 6"
Name: "win64net48"; Description: "Windows x64 .NET 4.8 Framework"
Name: "win86net48"; Description: "Windows x86 .NET 4.8 Framework"
Name: "win64net461"; Description: "Windows x64 .NET 4.6.1 Framework"
Name: "win86net461"; Description: "Windows x86 .NET 4.6.1 Framework"

[Components]
Name: "win64net9"; Description: "{#MyAppVerName}"; Types: win64net9
Name: "win86net9"; Description: "{#MyAppVerName}"; Types: win86net9
Name: "win64net8"; Description: "{#MyAppVerName}"; Types: win64net8
Name: "win86net8"; Description: "{#MyAppVerName}"; Types: win86net8
Name: "win64net7"; Description: "{#MyAppVerName}"; Types: win64net7
Name: "win86net7"; Description: "{#MyAppVerName}"; Types: win86net7
Name: "win64net6"; Description: "{#MyAppVerName}"; Types: win64net6
Name: "win86net6"; Description: "{#MyAppVerName}"; Types: win86net6
Name: "win64net48"; Description: "{#MyAppVerName}"; Types: win64net48
Name: "win86net48"; Description: "{#MyAppVerName}"; Types: win86net48
Name: "win64net461"; Description: "{#MyAppVerName}"; Types: win64net461
Name: "win86net461"; Description: "{#MyAppVerName}"; Types: win86net461
Name: "configuration"; Description: "Configuration Files"; Types: win86net461 win64net461 win86net48 win64net48 win86net6 win64net6 win86net7 win64net7 win86net8 win64net8 win86net9 win64net9
Name: "schemas"; Description: "MTConnect XSD Schema Files"; Types: win86net461 win64net461 win86net48 win64net48 win86net6 win64net6 win86net7 win64net7 win86net8 win64net8 win86net9 win64net9
Name: "styles"; Description: "XSL Stylesheet Files"; Types: win86net461 win64net461 win86net48 win64net48 win86net6 win64net6 win86net7 win64net7 win86net8 win64net8 win86net9 win64net9


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{app}"; Permissions: everyone-full
Name: "{app}\logs"; Permissions: everyone-full
Name: "{app}\devices"; Permissions: everyone-full
Name: "{app}\styles"; Permissions: everyone-full
Name: "{app}\schemas"; Permissions: everyone-full

[Files]

; Program Files
Source: "[INPUT_PATH]\win-x64\net9.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net9;
Source: "[INPUT_PATH]\win-x86\net9.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net9;
Source: "[INPUT_PATH]\win-x64\net8.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net8;
Source: "[INPUT_PATH]\win-x86\net8.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net8;
Source: "[INPUT_PATH]\win-x64\net7.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net7;
Source: "[INPUT_PATH]\win-x86\net7.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net7;
Source: "[INPUT_PATH]\win-x64\net6.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net6;
Source: "[INPUT_PATH]\win-x86\net6.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net6;
Source: "[INPUT_PATH]\win-x64\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net48;
Source: "[INPUT_PATH]\win-x86\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net48;
Source: "[INPUT_PATH]\win-x64\net461\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win64net461;
Source: "[INPUT_PATH]\win-x86\net461\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Permissions: everyone-full; Components: win86net461;

Source: "[STYLES_PATH]\*"; DestDir: "{app}\styles"; Flags: ignoreversion; Permissions: everyone-full;
Source: "[SCHEMAS_PATH]\*"; DestDir: "{app}\schemas"; Flags: ignoreversion; Permissions: everyone-full;

Source: "{#LicenseFile}"; DestDir: "{app}"; Flags: ignoreversion; Permissions: everyone-full;
Source: "[BASE_PATH]\icons\{#IconFile}"; DestDir: "{app}"; Flags: ignoreversion; Permissions: everyone-full;

[Run]

; Install and Run Windows Service
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""stop""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Stopping Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""remove""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Uninstalling Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""install""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Installing Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""start""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Starting Windows Service...";

; Start Browser
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""http://localhost:5000?outputComments=true&indentOutput=true""" ; Flags: runhidden postinstall unchecked; Description: Open Probe in Web Browser;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""http://localhost:5000/current?outputComments=true&indentOutput=true""" ; Flags: runhidden postinstall unchecked; Description: Open Current in Web Browser;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""agent.config.yaml""" ; WorkingDir: {app}; Flags: runhidden postinstall unchecked; Description: Open Agent Configuration File;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""NLog.config""" ; WorkingDir: {app}; Flags: runhidden postinstall unchecked; Description: Open Log Configuration File;
Filename: {sys}\cmd.exe; Parameters: "/c explorer ""{app}""" ; Flags: runhidden postinstall unchecked; Description: Open Install Directory;


[UninstallRun]

; Stop Server Service
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""stop""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Stopping Windows Service...";
Filename: {sys}\cmd.exe; Parameters: "/c agent.exe ""remove""" ; WorkingDir: {app}; Flags: runhidden; StatusMsg: "Uninstalling Windows Service...";