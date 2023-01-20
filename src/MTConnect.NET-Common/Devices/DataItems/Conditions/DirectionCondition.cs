// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication of a fault associated with the direction of motion of a Structural Element
    /// </summary>
    public class DirectionCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "DIRECTION";
        public const string NameId = "dirCond";
        public new const string DescriptionText = "An indication of a fault associated with the direction of motion of a Structural Element";

        public override string TypeDescription => DescriptionText;


        public DirectionCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public DirectionCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}