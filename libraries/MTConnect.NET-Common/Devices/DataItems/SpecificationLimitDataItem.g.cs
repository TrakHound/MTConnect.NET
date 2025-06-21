// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605646964270_821694_3328

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits defining a range of values designating acceptable performance for a variable.**DEPRECATED** in *Version 2.5*. Replaced by  `SPECIFICATION_LIMITS`.
    /// </summary>
    public class SpecificationLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SPECIFICATION_LIMIT";
        public const string NameId = "specificationLimit";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Set of limits defining a range of values designating acceptable performance for a variable.**DEPRECATED** in *Version 2.5*. Replaced by  `SPECIFICATION_LIMITS`.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public SpecificationLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public SpecificationLimitDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}