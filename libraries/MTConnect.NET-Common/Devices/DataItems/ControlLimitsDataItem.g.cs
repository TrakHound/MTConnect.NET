// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1696080573204_6427_2384

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits used to indicate whether a process variable is stable and in control.
    /// </summary>
    public class ControlLimitsDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROL_LIMITS";
        public const string NameId = "controlLimits";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Set of limits used to indicate whether a process variable is stable and in control.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version23;       


        public ControlLimitsDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ControlLimitsDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}