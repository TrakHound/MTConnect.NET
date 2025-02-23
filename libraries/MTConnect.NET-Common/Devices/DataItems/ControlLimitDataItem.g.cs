// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605646894483_607516_3273

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits used to indicate whether a process variable is stable and in control.**DEPRECATED** in *Version 2.5*. Replaced by `CONTROL_LIMITS`.
    /// </summary>
    public class ControlLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROL_LIMIT";
        public const string NameId = "controlLimit";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Set of limits used to indicate whether a process variable is stable and in control.**DEPRECATED** in *Version 2.5*. Replaced by `CONTROL_LIMITS`.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public ControlLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ControlLimitDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}