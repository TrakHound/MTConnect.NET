// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication of the operational condition of the interlock function for an electronically controller chuck.
    /// </summary>
    public class ChuckInterlockCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "CHUCK_INTERLOCK";
        public const string NameId = "chuckInterlockCond";
        public new const string DescriptionText = "An indication of the operational condition of the interlock function for an electronically controller chuck.";

        public override string TypeDescription => DescriptionText;


        public ChuckInterlockCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ChuckInterlockCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}