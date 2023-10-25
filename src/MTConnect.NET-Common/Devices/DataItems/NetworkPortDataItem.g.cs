// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Number of the TCP/IP or UDP/IP port for the connection endpoint.
    /// </summary>
    public class NetworkPortDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "NETWORK_PORT";
        public const string NameId = "";
             
        public new const string DescriptionText = "Number of the TCP/IP or UDP/IP port for the connection endpoint.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public NetworkPortDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public NetworkPortDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}