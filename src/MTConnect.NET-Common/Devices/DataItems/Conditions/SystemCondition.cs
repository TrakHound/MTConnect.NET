// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
    /// </summary>
    public class SystemCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "SYSTEM";
        public const string NameId = "system";
        public new const string DescriptionText = "An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public SystemCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public SystemCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}