// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// General purpose indication associated with an electronic component of a piece of equipment or a controller that represents a fault that is not associated with the operator, program, or hardware.
    /// </summary>
    public class SystemDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "SYSTEM";
        public const string NameId = "system";
             
        public new const string DescriptionText = "General purpose indication associated with an electronic component of a piece of equipment or a controller that represents a fault that is not associated with the operator, program, or hardware.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public SystemDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public SystemDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}