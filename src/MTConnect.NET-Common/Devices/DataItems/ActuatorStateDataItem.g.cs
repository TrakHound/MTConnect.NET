// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public class ActuatorStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ACTUATOR_STATE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Operational state of an apparatus for moving or controlling a mechanism or system.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public ActuatorStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ActuatorStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}