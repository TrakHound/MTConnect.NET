// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;

namespace MTConnect.Services
{
    internal static class WindowsService
    {
        public static bool ServiceExists(string serviceName)
        {
            try
            {
                var services = ServiceController.GetServices();
                return services.Any(s => s.ServiceName == serviceName);
            }
            catch { }

            return false;
        }

        public static bool IsCompatible()
        {
#if NET5_0_OR_GREATER
            try
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
            catch { }

            return false;
#else
           return true;
#endif
        }

        public static bool IsUserAdministrator()
        {
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { }

            return false;
        }
    }
}