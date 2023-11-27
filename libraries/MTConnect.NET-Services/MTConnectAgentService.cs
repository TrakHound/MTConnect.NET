// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
        private const string DefaultServiceName = "MTConnect-Agent";
        private const string DefaultServiceDisplayName = "MTConnect Agent";
        private const string DefaultServiceDescription = "MTConnect Agent to provide access to device information";
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
            // Configuration File Path
            string configFile = null;
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args) LogInformation($"MTConnectAgentService : OnStart (Arguments) : {arg}");

                if (args.Length > 1) configFile = args[1];
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
                var filename = $"{Assembly.GetEntryAssembly().GetName().Name}.exe";
                var path = Path.Combine(dir, filename);

                var start = _serviceStart ? "auto" : "demand";

                // Create Service
                var cmd = $"/c sc create {_serviceName} BinPath=\"\\\"{path}\\\" run-service \\\"{configurationPath}\\\"\" start= {start} DisplayName= \"{_serviceDisplayName}\"";
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