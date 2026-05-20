// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace MTConnect.Services
{
    /// <summary>
    /// Class used to implement an MTConnect Adapter as a Windows Service
    /// </summary>
    [SupportedOSPlatform("windows")]
    public abstract class MTConnectAdapterService : ServiceBase
    {
        private const string DefaultServiceName = "MTConnect-Adapter";
        private const string DefaultServiceDisplayName = "MTConnect SHDR Adapter";
        private const string DefaultServiceDescription = "MTConnect Adapter using SHDR to communicate with MTConnect Agent(s)";
        private const bool DefaultServiceStart = true;

        private string _serviceName = DefaultServiceName;
        private string _serviceDisplayName = DefaultServiceDisplayName;
        private string _serviceDescription = DefaultServiceDescription;
        private bool _serviceStart = DefaultServiceStart;


        /// <summary>
        /// Raised when the service emits an informational log message.
        /// </summary>
        public EventHandler<string> LogInformationReceived { get; set; }

        /// <summary>
        /// Raised when the service emits a warning log message.
        /// </summary>
        public EventHandler<string> LogWarningReceived { get; set; }

        /// <summary>
        /// Raised when the service emits an error log message.
        /// </summary>
        public EventHandler<string> LogErrorReceived { get; set; }


        /// <summary>
        /// Initializes the Windows Service wrapper, optionally suffixing the service name with a label to allow multiple adapter instances.
        /// </summary>
        /// <param name="label">An optional suffix appended to the service name to distinguish multiple adapters.</param>
        /// <param name="name">The Windows Service name; defaults to the standard adapter service name when empty.</param>
        /// <param name="displayName">The Windows Service display name; defaults to the standard name when empty.</param>
        /// <param name="description">The Windows Service description; defaults to the standard description when empty.</param>
        /// <param name="autoStart">Whether the service starts automatically after installation.</param>
        public MTConnectAdapterService(string label = null, string name = null, string displayName = null, string description = null, bool autoStart = true)
        {
            _serviceName = !string.IsNullOrEmpty(name) ? name : DefaultServiceName;
            if (!string.IsNullOrEmpty(label)) _serviceName += "-" + label;
            _serviceDisplayName = !string.IsNullOrEmpty(displayName) ? displayName : DefaultServiceDisplayName;
            _serviceDescription = !string.IsNullOrEmpty(description) ? description : DefaultServiceDescription;
            _serviceStart = autoStart;
        }


        /// <summary>
        /// Starts the adapter when the Windows Service starts, reading the configuration-file path from the service arguments.
        /// </summary>
        /// <param name="args">The service start arguments.</param>
        protected override void OnStart(string[] args)
        {
            // Configuration File Path
            string configFile = null;
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args) LogInformation($"MTConnectAdapterService : OnStart (Arguments) : {arg}");

                if (args.Length > 1) configFile = args[1];
            }

            LogInformation("MTConnectAdapterService : OnStart : Service Starting");
            StartAdapter(configFile);
            LogInformation("MTConnectAdapterService : OnStart : Service Started");

            base.OnStart(args);
        }

        /// <summary>
        /// Stops the adapter when the Windows Service stops.
        /// </summary>
        protected override void OnStop()
        {
            LogInformation("MTConnectAdapterService : OnStop : Service Stopping");
            StopAdapter();
            LogInformation("MTConnectAdapterService : OnStop : Service Stopped");

            base.OnStop();
        }


        /// <summary>
        /// Hook for derived services to start the hosted adapter. The base implementation does nothing.
        /// </summary>
        /// <param name="configurationPath">The path to the adapter configuration file, or null for the default.</param>
        protected virtual void StartAdapter(string configurationPath) { }

        /// <summary>
        /// Hook for derived services to stop the hosted adapter. The base implementation does nothing.
        /// </summary>
        protected virtual void StopAdapter() { }


        /// <summary>
        /// Hook for derived services to handle an informational log message. The base implementation does nothing.
        /// </summary>
        /// <param name="message">The log message.</param>
        protected virtual void OnLogInformation(string message) { }

        /// <summary>
        /// Hook for derived services to handle a warning log message. The base implementation does nothing.
        /// </summary>
        /// <param name="message">The log message.</param>
        protected virtual void OnLogWarning(string message) { }

        /// <summary>
        /// Hook for derived services to handle an error log message. The base implementation does nothing.
        /// </summary>
        /// <param name="message">The log message.</param>
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


        /// <summary>
        /// Installs the adapter as a Windows Service when running on a compatible Windows platform.
        /// </summary>
        /// <param name="configurationPath">The path to the adapter configuration file, or null for the default.</param>
        public void InstallService(string configurationPath = null)
        {
            if (WindowsService.IsCompatible()) WindowsInstall(configurationPath);
        }

        /// <summary>
        /// Removes the previously installed Windows Service when running on a compatible Windows platform.
        /// </summary>
        public void RemoveService()
        {
            if (WindowsService.IsCompatible()) WindowsRemove();
        }

        /// <summary>
        /// Starts the installed Windows Service when running on a compatible Windows platform.
        /// </summary>
        public void StartService()
        {
            if (WindowsService.IsCompatible()) WindowsStart();
        }

        /// <summary>
        /// Stops the running Windows Service when running on a compatible Windows platform.
        /// </summary>
        public void StopService()
        {
            if (WindowsService.IsCompatible()) WindowsStop();
        }


        #region "Windows"

        /// <summary>
        /// Registers the adapter with the Windows Service Control Manager using the configured service identity.
        /// </summary>
        /// <param name="configurationPath">The path to the adapter configuration file, or null for the default.</param>
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

        /// <summary>
        /// Unregisters the adapter's Windows Service from the Service Control Manager.
        /// </summary>
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

        /// <summary>
        /// Starts the adapter's installed Windows Service through the Service Control Manager.
        /// </summary>
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

        /// <summary>
        /// Stops the adapter's running Windows Service through the Service Control Manager.
        /// </summary>
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