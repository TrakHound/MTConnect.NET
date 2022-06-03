// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
            try
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
            catch { }

            return false;
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
