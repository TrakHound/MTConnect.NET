// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Describes the way the axes will be associated to each other.
    /// </summary>
    public class AxisCouplingDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AXIS_COUPLING";
        public const string NameId = "axisCoupling";
        public new const string DescriptionText = "Describes the way the axes will be associated to each other.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public AxisCouplingDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public AxisCouplingDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
