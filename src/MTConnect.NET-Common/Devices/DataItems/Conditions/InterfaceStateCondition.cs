// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Conditions
{
    /// <summary>
    /// An indication of the operation condition of an Interface component.
    /// </summary>
    public class InterfaceStateCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "INTERFACE_STATE";
        public const string NameId = "interfaceStateCond";
        public new const string DescriptionText = "An indication of the operation condition of an Interface component.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public InterfaceStateCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public InterfaceStateCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}