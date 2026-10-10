// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// FOCAS function return codes used by the module.
    /// </summary>
    public static class FocasReturnCodes
    {
        /// <summary>Protocol error (Ethernet).</summary>
        public const short EW_PROTOCOL = -17;
        /// <summary>Socket error (Ethernet).</summary>
        public const short EW_SOCKET = -16;
        /// <summary>The FOCAS library (Fwlib32.dll / Fwlib64.dll or a series DLL) could not be loaded.</summary>
        public const short EW_NODLL = -15;
        /// <summary>Bus error.</summary>
        public const short EW_BUS = -11;
        /// <summary>System error.</summary>
        public const short EW_SYSTEM2 = -10;
        /// <summary>HSSB communication error.</summary>
        public const short EW_HSSB = -9;
        /// <summary>Invalid library handle.</summary>
        public const short EW_HANDLE = -8;
        /// <summary>Version mismatch between the CNC and the library.</summary>
        public const short EW_VERSION = -7;
        /// <summary>Unexpected error.</summary>
        public const short EW_UNEXP = -6;
        /// <summary>System error.</summary>
        public const short EW_SYSTEM = -5;
        /// <summary>The CNC is being reset or stopped.</summary>
        public const short EW_RESET = -2;
        /// <summary>The CNC is busy.</summary>
        public const short EW_BUSY = -1;
        /// <summary>Success.</summary>
        public const short EW_OK = 0;
        /// <summary>The function is not supported by the CNC.</summary>
        public const short EW_FUNC = 1;
        /// <summary>Data number error.</summary>
        public const short EW_NUMBER = 3;
        /// <summary>The required CNC option is not installed.</summary>
        public const short EW_NOOPT = 6;


        /// <summary>
        /// Returns whether a return code means the function is not available on this CNC.
        /// </summary>
        /// <param name="code">The return code.</param>
        /// <returns><c>true</c> for EW_FUNC, EW_NOOPT and EW_VERSION.</returns>
        public static bool IsUnsupported(short code) => code == EW_FUNC || code == EW_NOOPT || code == EW_VERSION;

        /// <summary>
        /// Returns whether a return code means the connection to the CNC was lost.
        /// </summary>
        /// <param name="code">The return code.</param>
        /// <returns><c>true</c> for negative codes other than EW_BUSY, EW_RESET and EW_VERSION.</returns>
        public static bool IsConnectionError(short code) => code < 0 && code != EW_BUSY && code != EW_RESET && code != EW_VERSION;
    }

    /// <summary>
    /// P/Invoke declarations for the FOCAS functions used by the module. Structures are read into
    /// byte buffers and decoded by <see cref="FocasClient"/>. The FOCAS libraries are not included
    /// and must be copied next to the application: Fwlib64.dll (64-bit) or Fwlib32.dll (32-bit),
    /// plus the series DLLs they load (ex. fwlib30i64.dll, fwlib0iD64.dll).
    /// </summary>
    internal static class FocasNative
    {
        private const string Fwlib32 = "Fwlib32.dll";
        private const string Fwlib64 = "Fwlib64.dll";

        private static readonly bool Is64Bit = IntPtr.Size == 8;


        public static short cnc_allclibhndl3(string ip, ushort port, int timeout, out ushort handle) =>
            Is64Bit ? X64.cnc_allclibhndl3(ip, port, timeout, out handle) : X86.cnc_allclibhndl3(ip, port, timeout, out handle);

        public static short cnc_freelibhndl(ushort handle) =>
            Is64Bit ? X64.cnc_freelibhndl(handle) : X86.cnc_freelibhndl(handle);

        public static short cnc_sysinfo(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_sysinfo(handle, buffer) : X86.cnc_sysinfo(handle, buffer);

        public static short cnc_statinfo(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_statinfo(handle, buffer) : X86.cnc_statinfo(handle, buffer);

        public static short cnc_exeprgname(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_exeprgname(handle, buffer) : X86.cnc_exeprgname(handle, buffer);

        public static short cnc_rdprgnum(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdprgnum(handle, buffer) : X86.cnc_rdprgnum(handle, buffer);

        public static short cnc_rdseqnum(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdseqnum(handle, buffer) : X86.cnc_rdseqnum(handle, buffer);

        public static short cnc_rdparam(ushort handle, short number, short axis, short length, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdparam(handle, number, axis, length, buffer) : X86.cnc_rdparam(handle, number, axis, length, buffer);

        public static short cnc_rdmacro(ushort handle, short number, short length, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdmacro(handle, number, length, buffer) : X86.cnc_rdmacro(handle, number, length, buffer);

        public static short cnc_rdaxisname(ushort handle, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdaxisname(handle, ref count, buffer) : X86.cnc_rdaxisname(handle, ref count, buffer);

        public static short cnc_rdposition(ushort handle, short type, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdposition(handle, type, ref count, buffer) : X86.cnc_rdposition(handle, type, ref count, buffer);

        public static short cnc_rdsvmeter(ushort handle, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdsvmeter(handle, ref count, buffer) : X86.cnc_rdsvmeter(handle, ref count, buffer);

        public static short cnc_rdspdlname(ushort handle, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdspdlname(handle, ref count, buffer) : X86.cnc_rdspdlname(handle, ref count, buffer);

        public static short cnc_rdspmeter(ushort handle, short type, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdspmeter(handle, type, ref count, buffer) : X86.cnc_rdspmeter(handle, type, ref count, buffer);

        public static short cnc_rdspeed(ushort handle, short type, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdspeed(handle, type, buffer) : X86.cnc_rdspeed(handle, type, buffer);

        public static short cnc_acts(ushort handle, byte[] buffer) =>
            Is64Bit ? X64.cnc_acts(handle, buffer) : X86.cnc_acts(handle, buffer);

        public static short cnc_rdtimer(ushort handle, short type, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdtimer(handle, type, buffer) : X86.cnc_rdtimer(handle, type, buffer);

        public static short cnc_rdalmmsg2(ushort handle, short type, ref short count, byte[] buffer) =>
            Is64Bit ? X64.cnc_rdalmmsg2(handle, type, ref count, buffer) : X86.cnc_rdalmmsg2(handle, type, ref count, buffer);

        public static short pmc_rdpmcrng(ushort handle, short addressType, short dataType, ushort start, ushort end, ushort length, byte[] buffer) =>
            Is64Bit ? X64.pmc_rdpmcrng(handle, addressType, dataType, start, end, length, buffer) : X86.pmc_rdpmcrng(handle, addressType, dataType, start, end, length, buffer);

        public static short cnc_getpath(ushort handle, out short path, out short maxPath) =>
            Is64Bit ? X64.cnc_getpath(handle, out path, out maxPath) : X86.cnc_getpath(handle, out path, out maxPath);

        public static short cnc_setpath(ushort handle, short path) =>
            Is64Bit ? X64.cnc_setpath(handle, path) : X86.cnc_setpath(handle, path);


        private static class X64
        {
            [DllImport(Fwlib64, CharSet = CharSet.Ansi)] public static extern short cnc_allclibhndl3(string ip, ushort port, int timeout, out ushort handle);
            [DllImport(Fwlib64)] public static extern short cnc_freelibhndl(ushort handle);
            [DllImport(Fwlib64)] public static extern short cnc_sysinfo(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_statinfo(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_exeprgname(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdprgnum(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdseqnum(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdparam(ushort handle, short number, short axis, short length, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdmacro(ushort handle, short number, short length, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdaxisname(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdposition(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdsvmeter(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdspdlname(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdspmeter(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdspeed(ushort handle, short type, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_acts(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdtimer(ushort handle, short type, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_rdalmmsg2(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short pmc_rdpmcrng(ushort handle, short addressType, short dataType, ushort start, ushort end, ushort length, [Out] byte[] buffer);
            [DllImport(Fwlib64)] public static extern short cnc_getpath(ushort handle, out short path, out short maxPath);
            [DllImport(Fwlib64)] public static extern short cnc_setpath(ushort handle, short path);
        }

        private static class X86
        {
            [DllImport(Fwlib32, CharSet = CharSet.Ansi)] public static extern short cnc_allclibhndl3(string ip, ushort port, int timeout, out ushort handle);
            [DllImport(Fwlib32)] public static extern short cnc_freelibhndl(ushort handle);
            [DllImport(Fwlib32)] public static extern short cnc_sysinfo(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_statinfo(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_exeprgname(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdprgnum(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdseqnum(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdparam(ushort handle, short number, short axis, short length, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdmacro(ushort handle, short number, short length, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdaxisname(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdposition(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdsvmeter(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdspdlname(ushort handle, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdspmeter(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdspeed(ushort handle, short type, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_acts(ushort handle, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdtimer(ushort handle, short type, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_rdalmmsg2(ushort handle, short type, ref short count, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short pmc_rdpmcrng(ushort handle, short addressType, short dataType, ushort start, ushort end, ushort length, [Out] byte[] buffer);
            [DllImport(Fwlib32)] public static extern short cnc_getpath(ushort handle, out short path, out short maxPath);
            [DllImport(Fwlib32)] public static extern short cnc_setpath(ushort handle, short path);
        }
    }
}
