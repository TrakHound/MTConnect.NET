// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Status of the Component.**DEPRECATED** in *Version 1.1.0*.
    /// </summary>
    public class PowerStatusDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "POWER_STATUS";
        public const string NameId = "powerStatus";
             
        public new const string DescriptionText = "Status of the Component.**DEPRECATED** in *Version 1.1.0*.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public PowerStatusDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PowerStatusDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}