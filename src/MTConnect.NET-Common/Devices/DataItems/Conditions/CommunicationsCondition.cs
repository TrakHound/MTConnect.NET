// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication that the piece of equipment has experienced a communications failure.
    /// </summary>
    public class CommunicationsCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "COMMUNICATIONS";
        public const string NameId = "comms";
        public new const string DescriptionText = "An indication that the piece of equipment has experienced a communications failure.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public CommunicationsCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public CommunicationsCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}