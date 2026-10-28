// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// <see cref="IFocasClient"/> implemented with the Fanuc FOCAS libraries over Ethernet.
    /// A FOCAS library handle is only valid in the thread that opened it, so every call is
    /// run on a dedicated thread owned by this client.
    /// </summary>
    public class FocasClient : IFocasClient
    {
        private const int MaxAxes = 32;
        private const int MaxSpindles = 8;
        private const int MaxAlarms = 10;

        private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
        private readonly Thread _thread;
        private ushort _handle;
        private bool _connected;
        private bool _missingLibraryLogged;


        /// <inheritdoc/>
        public bool IsConnected => _connected;

        /// <summary>
        /// Raised when the client emits a log message.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new client and starts its FOCAS thread.
        /// </summary>
        public FocasClient()
        {
            _thread = new Thread(Run) { IsBackground = true, Name = "FOCAS" };
            _thread.Start();
        }


        /// <inheritdoc/>
        public short Connect(string host, int port, int timeoutSeconds)
        {
            return Invoke(() =>
            {
                FreeHandle();

                ushort handle;
                var code = FocasNative.cnc_allclibhndl3(host, (ushort)port, Math.Max(1, timeoutSeconds), out handle);
                if (code == FocasReturnCodes.EW_OK)
                {
                    _handle = handle;
                    _connected = true;
                }

                return code;
            });
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            Invoke(() =>
            {
                FreeHandle();
                return FocasReturnCodes.EW_OK;
            });
        }

        /// <inheritdoc/>
        public short ReadSystemInfo(out FocasSystemInfo info)
        {
            FocasSystemInfo result = null;
            var code = Invoke(() =>
            {
                var buffer = new byte[18];
                var ret = FocasNative.cnc_sysinfo(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new FocasSystemInfo
                    {
                        AddInfo = BitConverter.ToInt16(buffer, 0),
                        MaxAxis = BitConverter.ToInt16(buffer, 2),
                        CncType = ReadString(buffer, 4, 2),
                        MtType = ReadString(buffer, 6, 2),
                        Series = ReadString(buffer, 8, 4),
                        Version = ReadString(buffer, 12, 4)
                    };
                }
                return ret;
            });

            info = result;
            return code;
        }

        /// <inheritdoc/>
        public short GetPath(out short path, out short maxPath)
        {
            short p = 0, max = 0;
            var code = Invoke(() => FocasNative.cnc_getpath(_handle, out p, out max));
            path = p;
            maxPath = max;
            return code;
        }

        /// <inheritdoc/>
        public short SetPath(short path)
        {
            return Invoke(() => FocasNative.cnc_setpath(_handle, path));
        }

        /// <inheritdoc/>
        public short ReadStatus(out FocasStatus status)
        {
            FocasStatus result = null;
            var code = Invoke(() =>
            {
                var buffer = new byte[18];
                var ret = FocasNative.cnc_statinfo(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new FocasStatus
                    {
                        Mode = BitConverter.ToInt16(buffer, 4),
                        Run = BitConverter.ToInt16(buffer, 6),
                        Motion = BitConverter.ToInt16(buffer, 8),
                        Mstb = BitConverter.ToInt16(buffer, 10),
                        Emergency = BitConverter.ToInt16(buffer, 12),
                        Alarm = BitConverter.ToInt16(buffer, 14),
                        Edit = BitConverter.ToInt16(buffer, 16)
                    };
                }
                return ret;
            });

            status = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadProgramName(out string name)
        {
            string result = null;
            var code = Invoke(() =>
            {
                var buffer = new byte[40];
                var ret = FocasNative.cnc_exeprgname(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK) result = ReadString(buffer, 0, 36);
                return ret;
            });

            name = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadProgramNumber(out int running, out int main)
        {
            int r = 0, m = 0;
            var code = Invoke(() =>
            {
                var buffer = new byte[12];
                var ret = FocasNative.cnc_rdprgnum(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    r = BitConverter.ToInt32(buffer, 4);
                    m = BitConverter.ToInt32(buffer, 8);
                }
                return ret;
            });

            running = r;
            main = m;
            return code;
        }

        /// <inheritdoc/>
        public short ReadSequenceNumber(out int number)
        {
            int n = 0;
            var code = Invoke(() =>
            {
                var buffer = new byte[8];
                var ret = FocasNative.cnc_rdseqnum(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK) n = BitConverter.ToInt32(buffer, 4);
                return ret;
            });

            number = n;
            return code;
        }

        /// <inheritdoc/>
        public short ReadParameter(short number, out int value)
        {
            int v = 0;
            var code = Invoke(() =>
            {
                // IODBPSD: datano (2), type (2), ldata (4)
                var buffer = new byte[8];
                var ret = FocasNative.cnc_rdparam(_handle, number, 0, 8, buffer);
                if (ret == FocasReturnCodes.EW_OK) v = BitConverter.ToInt32(buffer, 4);
                return ret;
            });

            value = v;
            return code;
        }

        /// <inheritdoc/>
        public short ReadMacro(short number, out double? value)
        {
            double? v = null;
            var code = Invoke(() =>
            {
                // ODBM: datano (2), dummy (2), mcr_val (4), dec_val (2)
                var buffer = new byte[12];
                var ret = FocasNative.cnc_rdmacro(_handle, number, 10, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    var data = BitConverter.ToInt32(buffer, 4);
                    var dec = BitConverter.ToInt16(buffer, 8);
                    if (!(data == 0 && dec == -1)) v = Scale(data, dec); // 0 / -1 = vacant
                }
                return ret;
            });

            value = v;
            return code;
        }

        /// <inheritdoc/>
        public short ReadAxisNames(out string[] names)
        {
            string[] result = null;
            var code = Invoke(() =>
            {
                short count = MaxAxes;
                var buffer = new byte[MaxAxes * 2];
                var ret = FocasNative.cnc_rdaxisname(_handle, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new string[Math.Min((int)count, MaxAxes)];
                    for (var i = 0; i < result.Length; i++) result[i] = ReadName(buffer, i * 2, 2);
                }
                return ret;
            });

            names = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadPositions(out FocasPosition[] positions)
        {
            FocasPosition[] result = null;
            var code = Invoke(() =>
            {
                // ODBPOS (48 bytes): abs, mach, rel, dist POSELM (12 bytes each)
                short count = MaxAxes;
                var buffer = new byte[MaxAxes * 48];
                var ret = FocasNative.cnc_rdposition(_handle, -1, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new FocasPosition[Math.Min((int)count, MaxAxes)];
                    for (var i = 0; i < result.Length; i++)
                    {
                        var offset = i * 48;
                        result[i] = new FocasPosition
                        {
                            Absolute = ReadPositionElement(buffer, offset),
                            Machine = ReadPositionElement(buffer, offset + 12),
                            Relative = ReadPositionElement(buffer, offset + 24),
                            DistanceToGo = ReadPositionElement(buffer, offset + 36),
                            Unit = BitConverter.ToInt16(buffer, offset + 12 + 6),
                            Name = ReadName(buffer, offset + 12 + 10, 2)
                        };
                    }
                }
                return ret;
            });

            positions = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadServoLoads(out FocasLoad[] loads)
        {
            FocasLoad[] result = null;
            var code = Invoke(() =>
            {
                // ODBSVLOAD: LOADELM (12 bytes) per axis
                short count = MaxAxes;
                var buffer = new byte[MaxAxes * 12];
                var ret = FocasNative.cnc_rdsvmeter(_handle, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK) result = ReadLoads(buffer, count, 12, MaxAxes);
                return ret;
            });

            loads = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadSpindleNames(out string[] names)
        {
            string[] result = null;
            var code = Invoke(() =>
            {
                short count = MaxSpindles;
                var buffer = new byte[MaxSpindles * 4];
                var ret = FocasNative.cnc_rdspdlname(_handle, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new string[Math.Min((int)count, MaxSpindles)];
                    for (var i = 0; i < result.Length; i++) result[i] = ReadName(buffer, i * 4, 4);
                }
                return ret;
            });

            names = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadSpindleLoads(out FocasLoad[] loads)
        {
            FocasLoad[] result = null;
            var code = Invoke(() =>
            {
                // ODBSPLOAD (24 bytes): spload, spspeed LOADELM per spindle
                short count = MaxSpindles;
                var buffer = new byte[MaxSpindles * 24];
                var ret = FocasNative.cnc_rdspmeter(_handle, 0, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK) result = ReadLoads(buffer, count, 24, MaxSpindles);
                return ret;
            });

            loads = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadSpeed(out FocasSpeed speed)
        {
            FocasSpeed result = null;
            var code = Invoke(() =>
            {
                // ODBSPEED: actf, acts SPEEDELM (12 bytes each: data, dec, unit, disp, name, suff)
                var buffer = new byte[24];
                var ret = FocasNative.cnc_rdspeed(_handle, -1, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new FocasSpeed
                    {
                        Feedrate = Scale(BitConverter.ToInt32(buffer, 0), BitConverter.ToInt16(buffer, 4)),
                        FeedrateUnit = BitConverter.ToInt16(buffer, 6),
                        SpindleSpeed = Scale(BitConverter.ToInt32(buffer, 12), BitConverter.ToInt16(buffer, 16))
                    };
                }
                return ret;
            });

            speed = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadSpindleSpeed(out int rpm)
        {
            int v = 0;
            var code = Invoke(() =>
            {
                // ODBACT: dummy[2] (4), data (4)
                var buffer = new byte[8];
                var ret = FocasNative.cnc_acts(_handle, buffer);
                if (ret == FocasReturnCodes.EW_OK) v = BitConverter.ToInt32(buffer, 4);
                return ret;
            });

            rpm = v;
            return code;
        }

        /// <inheritdoc/>
        public short ReadTimer(short type, out double seconds)
        {
            double v = 0;
            var code = Invoke(() =>
            {
                // IODBTIME: minute (4), msec (4)
                var buffer = new byte[8];
                var ret = FocasNative.cnc_rdtimer(_handle, type, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    v = (BitConverter.ToInt32(buffer, 0) * 60.0) + (BitConverter.ToInt32(buffer, 4) / 1000.0);
                }
                return ret;
            });

            seconds = v;
            return code;
        }

        /// <inheritdoc/>
        public short ReadAlarms(out FocasAlarm[] alarms)
        {
            FocasAlarm[] result = null;
            var code = Invoke(() =>
            {
                // ODBALMMSG2 (76 bytes): alm_no (4), type (2), axis (2), dummy (2), msg_len (2), alm_msg (64)
                short count = MaxAlarms;
                var buffer = new byte[MaxAlarms * 76];
                var ret = FocasNative.cnc_rdalmmsg2(_handle, -1, ref count, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    var list = new List<FocasAlarm>();
                    for (var i = 0; i < Math.Min((int)count, MaxAlarms); i++)
                    {
                        var offset = i * 76;
                        var length = Math.Max((short)0, Math.Min(BitConverter.ToInt16(buffer, offset + 10), (short)64));
                        list.Add(new FocasAlarm
                        {
                            Number = BitConverter.ToInt32(buffer, offset),
                            Type = BitConverter.ToInt16(buffer, offset + 4),
                            Axis = BitConverter.ToInt16(buffer, offset + 6),
                            Message = ReadString(buffer, offset + 12, length)
                        });
                    }
                    result = list.ToArray();
                }
                return ret;
            });

            alarms = result;
            return code;
        }

        /// <inheritdoc/>
        public short ReadPmc(short addressType, ushort start, ushort end, out byte[] data)
        {
            byte[] result = null;
            var code = Invoke(() =>
            {
                // IODBPMC: type_a (2), type_d (2), datano_s (2), datano_e (2), cdata[]
                var count = end - start + 1;
                var length = (ushort)(8 + count);
                var buffer = new byte[length];
                var ret = FocasNative.pmc_rdpmcrng(_handle, addressType, 0, start, end, length, buffer);
                if (ret == FocasReturnCodes.EW_OK)
                {
                    result = new byte[count];
                    Array.Copy(buffer, 8, result, 0, count);
                }
                return ret;
            });

            data = result;
            return code;
        }

        /// <summary>
        /// Releases the library handle and stops the FOCAS thread.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            _queue.CompleteAdding();
        }


        /// <summary>
        /// Runs <paramref name="action"/> on the FOCAS thread (directly when already on it) and waits for it to finish.
        /// FOCAS calls made inside it run without a thread hand-off each.
        /// </summary>
        public void RunOnFocasThread(Action action)
        {
            if (action == null) return;

            Exception error = null;
            Invoke(() =>
            {
                try { action(); }
                catch (Exception ex) { error = ex; }
                return FocasReturnCodes.EW_OK;
            });

            if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }

        private short Invoke(Func<short> function)
        {
            if (Thread.CurrentThread == _thread) return Execute(function);

            short result = FocasReturnCodes.EW_UNEXP;
            using (var done = new ManualResetEventSlim())
            {
                try
                {
                    _queue.Add(() =>
                    {
                        result = Execute(function);
                        done.Set();
                    });
                }
                catch (InvalidOperationException)
                {
                    return FocasReturnCodes.EW_HANDLE; // Disposed
                }

                done.Wait();
            }

            return result;
        }

        private short Execute(Func<short> function)
        {
            try
            {
                return function();
            }
            catch (DllNotFoundException ex)
            {
                if (!_missingLibraryLogged)
                {
                    var library = IntPtr.Size == 8 ? "Fwlib64.dll" : "Fwlib32.dll";
                    Log(MTConnectLogLevel.Error, $"FOCAS library not found. Copy {library} and the Fanuc series libraries next to the application : {ex.Message}");
                    _missingLibraryLogged = true;
                }
                return FocasReturnCodes.EW_NODLL;
            }
            catch (EntryPointNotFoundException)
            {
                return FocasReturnCodes.EW_FUNC;
            }
            catch (Exception ex)
            {
                Log(MTConnectLogLevel.Warning, $"FOCAS Call Failed : {ex.Message}");
                return FocasReturnCodes.EW_UNEXP;
            }
        }

        private void Run()
        {
            foreach (var action in _queue.GetConsumingEnumerable()) action();
        }

        private void FreeHandle()
        {
            if (_connected)
            {
                try { FocasNative.cnc_freelibhndl(_handle); }
                catch { }
            }

            _connected = false;
            _handle = 0;
        }

        private void Log(MTConnectLogLevel level, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, level, message);
        }


        private static double ReadPositionElement(byte[] buffer, int offset)
        {
            // POSELM: data (4), dec (2), unit (2), disp (2), name (1), suff (1)
            return Scale(BitConverter.ToInt32(buffer, offset), BitConverter.ToInt16(buffer, offset + 4));
        }

        private static FocasLoad[] ReadLoads(byte[] buffer, short count, int stride, int max)
        {
            // LOADELM: data (4), dec (2), unit (2), name (1), suff1 (1), suff2 (1), reserve (1)
            var loads = new FocasLoad[Math.Min((int)count, max)];
            for (var i = 0; i < loads.Length; i++)
            {
                var offset = i * stride;
                loads[i] = new FocasLoad
                {
                    Value = Scale(BitConverter.ToInt32(buffer, offset), BitConverter.ToInt16(buffer, offset + 4)),
                    Name = ReadName(buffer, offset + 8, 3)
                };
            }
            return loads;
        }

        internal static double Scale(int data, short dec)
        {
            return dec > 0 ? data / Math.Pow(10, dec) : data;
        }

        internal static string ReadName(byte[] buffer, int offset, int length)
        {
            // Axis / spindle name followed by suffix characters (0 or ' ' = none)
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var c = buffer[offset + i];
                if (c > 0x20 && c < 0x7F) builder.Append((char)c);
            }
            return builder.ToString();
        }

        private static string ReadString(byte[] buffer, int offset, int length)
        {
            var end = offset;
            while (end < offset + length && buffer[end] != 0) end++;
            return Encoding.ASCII.GetString(buffer, offset, end - offset).Trim();
        }
    }
}
