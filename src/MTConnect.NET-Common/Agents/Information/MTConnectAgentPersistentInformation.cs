// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Agents.Information
{
    public class MTConnectAgentPersistentInformation : MTConnectAgentInformation
    {
        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("deviceModelChangeTime")]
        public long DeviceModelChangeTime { get; set; }


        public MTConnectAgentPersistentInformation()
        {
            InstanceId = UnixDateTime.Now;
        }
    }
}
