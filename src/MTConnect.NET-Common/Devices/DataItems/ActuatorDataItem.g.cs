// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of a fault associated with an actuator.
    /// </summary>
    public class ActuatorDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "ACTUATOR";
        public const string NameId = "";
             
        public new const string DescriptionText = "Indication of a fault associated with an actuator.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public ActuatorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ActuatorDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}