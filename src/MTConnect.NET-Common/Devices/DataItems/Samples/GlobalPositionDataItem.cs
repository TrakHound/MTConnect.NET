// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Position in three-dimensional space.
    /// </summary>
    public class GlobalPositionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "GLOABAL_POSITION";
        public const string NameId = "globalPosition";
        public const string DefaultUnits = Devices.Units.MILLIMETER;
        public new const string DescriptionText = "Position in three-dimensional space.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MaximumVersion => MTConnectVersions.Version10;


        public GlobalPositionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public GlobalPositionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
