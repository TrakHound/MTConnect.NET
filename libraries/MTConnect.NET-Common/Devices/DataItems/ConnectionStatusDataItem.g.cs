// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Status of the connection between an adapter and an agent.
    /// </summary>
    public class ConnectionStatusDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONNECTION_STATUS";
        public const string NameId = "connectionStatus";
             
        public new const string DescriptionText = "Status of the connection between an adapter and an agent.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public ConnectionStatusDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ConnectionStatusDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}