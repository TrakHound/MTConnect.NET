// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System.Text;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model Input that carries a Device definition for reporting to an Agent.
    /// </summary>
    public class DeviceInput : IDeviceInput
    {
        private static readonly Encoding _utf8 = new UTF8Encoding();

        private byte[] _changeId;
        private byte[] _changeIdWithTimestamp;


        /// <summary>
        /// The UUID or Name of the Device
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// The Device to add
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the Device was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// An MD5 Hash of the Device that can be used for comparison
        /// </summary>
        public byte[] ChangeId
        {
            get
            {
                if (_changeId == null) _changeId = CreateChangeId(this);
                return _changeId;
            }
        }

        /// <summary>
        /// An MD5 Hash of the Device including the Timestamp that can be used for comparison
        /// </summary>
        public byte[] ChangeIdWithTimestamp
        {
            get
            {
                if (_changeIdWithTimestamp == null) _changeIdWithTimestamp = CreateChangeId(this);
                return _changeIdWithTimestamp;
            }
        }


        /// <summary>
        /// Initializes a new Device Input from a Device, using the Device UUID as the key.
        /// </summary>
        /// <param name="device">The Device to report. A <c>null</c> argument leaves the new instance empty.</param>
        public DeviceInput(IDevice device)
        {
            if (device != null)
            {
                DeviceKey = device.Uuid;
                Device = device;
            }
        }

        /// <summary>
        /// Initializes a new Device Input from a Device with an explicit Device key.
        /// </summary>
        /// <param name="deviceKey">The UUID or Name used to identify the Device.</param>
        /// <param name="device">The Device to report. A <c>null</c> argument leaves the new instance empty.</param>
        public DeviceInput(string deviceKey, IDevice device)
        {
            if (device != null)
            {
                DeviceKey = deviceKey;
                Device = device;
            }
        }

        /// <summary>
        /// Initializes a new Device Input by copying the Device key, Device, and timestamp from an existing Device Input.
        /// </summary>
        /// <param name="device">The source Device Input to copy; a <c>null</c> argument leaves the new instance empty.</param>
        public DeviceInput(IDeviceInput device)
        {
            if (device != null)
            {
                DeviceKey = device.DeviceKey;
                Device = device.Device;
                Timestamp = device.Timestamp;
            }
        }


        private static byte[] CreateChangeId(IDeviceInput deviceInput)
        {
            if (deviceInput != null && deviceInput.Device != null)
            {
                var sb = new StringBuilder();

                // Add DeviceKey (if specified)
                if (!string.IsNullOrEmpty(deviceInput.DeviceKey)) sb.Append($"{deviceInput.DeviceKey}:::");

                // Add Device Hash
                sb.Append($"{deviceInput.Device.GenerateHash()}::");

                // Get Bytes from StringBuilder
                char[] a = new char[sb.Length];
                sb.CopyTo(0, a, 0, sb.Length);

                // Convert StringBuilder result to UTF8 MD5 Bytes
                return _utf8.GetBytes(a).ToMD5HashBytes();
            }

            return null;
        }
    }
}
