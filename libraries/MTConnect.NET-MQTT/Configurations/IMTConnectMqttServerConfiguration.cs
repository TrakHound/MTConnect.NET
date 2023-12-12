// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttServerConfiguration
    {
        /// <summary>
        /// Sets the Interval (in milliseconds) to send Current messages at
        /// </summary>
        int CurrentInterval { get; }

        /// <summary>
        /// Sets the Interval (in milliseconds) to send Sample messages at
        /// </summary>
        int SampleInterval { get; }
    }
}
