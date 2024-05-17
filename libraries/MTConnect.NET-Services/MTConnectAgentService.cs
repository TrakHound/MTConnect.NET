// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace MTConnect.Services
{
    /// <summary>
    /// Class used to implement an MTConnect Agent as a Windows Service
    /// </summary>
    public abstract class MTConnectAgentService : ServiceBase
    {
        private const string DefaultServiceName = "MTConnect.NET-Agent";
        private const string DefaultServiceDisplayName = "MTConnect.NET Agent";
        private const string DefaultServiceDescription = ".NET MTConnect Agent to provide access to equipment information";
        private const bool DefaultServiceStart = true;

        private string _serviceName = DefaultServiceName;
        private string _serviceDisplayName = DefaultServiceDisplayName;
        private string _serviceDescription = DefaultServiceDescription;
        private bool _serviceStart = DefaultServiceStart;


        public EventHandler<string> LogInformationReceived { get; set; }

        public EventHandler<string> LogWarningReceived { get; set; }

        public EventHandler<string> LogErrorReceived { get; set; }


        public MTConnectAgentService(string name = null, string displayName = null, string description = null, bool autoStart = true)
        { 
            _serviceName = !string.IsNullOrEmpty(name) ? name : DefaultServiceName;
            _serviceDisplayName = !string.IsNullOrEmpty(displayName) ? displayName : DefaultServiceDisplayName;
            _serviceDescription = !string.IsNullOrEmpty(description) ? description : DefaultServiceDescription;
            _serviceStart = autoStart;
        }


        protected override void OnStart(string[] args)
        {
            // Read Command Line Args manually (they are not passed in the args variable)
            // unless specified in the Start Parameters when creating the Windows Service
            // https://learn.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicebase.onstart?view=net-8.0#remarks
            var commandArgs = Environment.GetCommandLineArgs();

            // Configuration File Path
            string configFile = null;
            if (commandArgs != null && commandArgs.Length > 1)
            {
                foreach (var arg in commandArgs) LogInformation($"MTConnectAgentService : OnStart (Arguments) : {arg}");

                if (commandArgs.Length > 2) configFile = commandArgs[2];
            }

            LogInformation("MTConnectAgentService : OnStart : Service Starting");
            StartAgent(configFile);
            LogInformation("MTConnectAgentService : OnStart : Service Started");

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            LogInformation("MTConnectAgentService : OnStop : Service Stopping");
            StopAgent();
            LogInformation("MTConnectAgentService : OnStop : Service Stopped");

            base.OnStop();
        }


        protected virtual void StartAgent(string configurationPath) { }

        protected virtual void StopAgent() { }


        protected virtual void OnLogInformation(string message) { }

        protected virtual void OnLogWarning(string message) { }

        protected virtual void OnLogError(string message) { }


        private void LogInformation(string message)
        {
            OnLogInformation(message);
            if (LogInformationReceived != null) LogInformationReceived.Invoke(this, message);
        }

        private void LogWarning(string message)
        {
            OnLogWarning(message);
            if (LogWarningReceived != null) LogWarningReceived.Invoke(this, message);
        }

        private void LogError(string message)
        {
            OnLogError(message);
            if (LogErrorReceived != null) LogErrorReceived.Invoke(this, message);
        }


        public void InstallService(string configurationPath = null)
        {
            if (WindowsService.IsCompatible()) WindowsInstall(configurationPath);
        }

        public void RemoveService()
        {
            if (WindowsService.IsCompatible()) WindowsRemove();
        }

        public void StartService()
        {
            if (WindowsService.IsCompatible()) WindowsStart();
        }

        public void StopService()
        {
            if (WindowsService.IsCompatible()) WindowsStop();
        }


        #region "Windows"

        public void WindowsInstall(string configurationPath = null)
        {
            if (WindowsService.IsUserAdministrator())
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;

                // Set Executable Path
                var exeFilename = $"{Assembly.GetEntryAssembly().GetName().Name}.exe";
                var exePath = Path.Combine(dir, exeFilename);

                // Set Configuration Path
                var configPath = configurationPath;
                if (!string.IsNullOrEmpty(configPath))
                {
                    if (!Path.IsPathRooted(configPath))
                    {
                        configPath = Path.Combine(dir, configPath);
                    }
                }

                var start = _serviceStart ? "auto" : "demand";

                // Create Service
                var cmd = $"/c sc create {_serviceName} BinPath= \"\\\"{exePath}\\\" run-service \\\"{configPath}\\\"\" start= {start} DisplayName= \"{_serviceDisplayName}\"";
                if (RunWindowsCommand(cmd))
                {
                    LogInformation($"Install Service : Windows : ({_serviceName}) Service Created Successfully");

                    cmd = $"/c sc description {_serviceName} \"{_serviceDescription}\"";
                    if (RunWindowsCommand(cmd))
                    {
                        LogInformation($"Install Service : Windows : ({_serviceName}) Service Description Set Successfully");


                    }
                    else
                    {
                        LogError($"Install Service : Windows : Set Description ({_serviceName}) Service Failed");
                    }
                }
                else
                {
                    LogError($"Install Service : Windows : Create ({_serviceName}) Service Failed");
                }
            }
            else
            {
                LogError("Install Service : Windows : Permission Denied");
            }
        }

        public void WindowsRemove()
        {
            if (WindowsService.IsUserAdministrator())
            {
                if (WindowsService.ServiceExists(_serviceName))
                {
                    var cmd = $"/c sc delete {_serviceName}";
                    if (RunWindowsCommand(cmd))
                    {
                        LogInformation($"Remove Service : Windows : ({_serviceName}) Service Removed Successfully");
                    }
                    else
                    {
                        LogError($"Remove Service : Windows : Remove ({_serviceName}) Service Failed");
                    }
                }
                else
                {
                    LogInformation($"Start Service : Windows : ({_serviceName}) Service Doesn't Exist");
                }
            }
            else
            {
                LogError("Remove Service : Windows : Permission Denied");
            }
        }

        public void WindowsStart()
        {
            if (WindowsService.IsUserAdministrator())
            {
                if (WindowsService.ServiceExists(_serviceName))
                {
                    var cmd = $"/c sc start {_serviceName}";
                    if (RunWindowsCommand(cmd))
                    {
                        LogInformation($"Start Service : Windows : ({_serviceName}) Service Started Successfully");
                    }
                    else
                    {
                        LogError($"Start Service : Windows : Start ({_serviceName}) Service Failed");
                    }
                }
                else
                {
                    LogInformation($"Start Service : Windows : ({_serviceName}) Service Doesn't Exist");
                }
            }
            else
            {
                LogError("Start Service : Windows : Permission Denied");
            }
        }

        public void WindowsStop()
        {
            if (WindowsService.IsUserAdministrator())
            {
                if (WindowsService.ServiceExists(_serviceName))
                {
                    var cmd = $"/c sc stop {_serviceName}";
                    if (RunWindowsCommand(cmd))
                    {
                        LogInformation($"Stop Service : Windows : ({_serviceName}) Service Stopped Successfully");
                    }
                    else
                    {
                        LogError($"Stop Service : Windows : Stop ({_serviceName}) Service Failed");
                    }
                }
                else
                {
                    LogInformation($"Stop Service : Windows : ({_serviceName}) Service Doesn't Exist");
                }
            }
            else
            {
                LogError("Stop Service : Windows : Permission Denied");
            }
        }


        private bool RunWindowsCommand(string cmd)
        {
            var exe = "cmd.exe";

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.FileName = exe;
                startInfo.Arguments = cmd;

                LogInformation($"RunWindowsCommand() : {exe} {cmd}");

                var process = Process.Start(startInfo);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                LogInformation($"RunWindowsCommand() : {output}");

                return true;
            }
            catch (Exception ex)
            {
                LogError($"RunWindowsCommand() : Exception : {ex.Message}");
            }

            return false;
        }

        #endregion
    }
}