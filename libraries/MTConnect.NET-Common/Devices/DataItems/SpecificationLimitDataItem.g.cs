// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    public class SpecificationLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SPECIFICATION_LIMIT";
        public const string NameId = "specificationLimit";
             
        public new const string DescriptionText = "Set of limits defining a range of values designating acceptable performance for a variable.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public SpecificationLimitDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public SpecificationLimitDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}