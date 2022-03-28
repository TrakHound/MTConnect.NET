// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// A three space angular rotation relative to a coordinate system.
    /// </summary>
    public class RotationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ROTATION";
        public const string NameId = "rotation";
        public new const string DescriptionText = "A three space angular rotation relative to a coordinate system.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public RotationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public RotationDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
