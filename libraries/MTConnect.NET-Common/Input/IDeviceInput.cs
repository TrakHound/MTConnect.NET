// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Input
{
    public interface IDeviceInput
    {
        /// <summary>
        /// The UUID or Name of the Device
        /// </summary>
        string DeviceKey { get; }

        /// <summary>
        /// The Device to add
        /// </summary>
        IDevice Device { get; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that will be recorded as DeviceModelChangeTime
        /// </summary>
        long Timestamp { get; }

        /// <summary>
        /// An MD5 Hash of the Device that can be used for comparison
        /// </summary>
        byte[] ChangeId { get; }

        /// <summary>
        /// An MD5 Hash of the Device including the Timestamp that can be used for comparison
        /// </summary>
        byte[] ChangeIdWithTimestamp { get; }
    }
}
