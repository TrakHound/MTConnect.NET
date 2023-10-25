// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication that the value of the data associated with a measured value or a calculation is outside of an expected range.
    /// </summary>
    public class DataRangeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "DATA_RANGE";
        public const string NameId = "";
             
        public new const string DescriptionText = "Indication that the value of the data associated with a measured value or a calculation is outside of an expected range.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public DataRangeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public DataRangeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}