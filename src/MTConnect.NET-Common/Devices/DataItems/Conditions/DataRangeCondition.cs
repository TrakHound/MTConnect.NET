// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication that the value of the data associated with a measured value or a calculation is outside of an expected range.
    /// </summary>
    public class DataRangeCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "DATA_RANGE";
        public const string NameId = "dataRange";
        public new const string DescriptionText = "An indication that the value of the data associated with a measured value or a calculation is outside of an expected range.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public DataRangeCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DataRangeCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}