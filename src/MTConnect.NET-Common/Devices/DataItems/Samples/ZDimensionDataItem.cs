// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Measured dimension of an entity relative to the Z direction of the referenced coordinate system.
    /// </summary>
    public class ZDimensionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "Z_DIMENSION";
        public const string NameId = "zDim";
        public const string DefaultUnits = Devices.Units.MILLIMETER;
        public new const string DescriptionText = "Measured dimension of an entity relative to the Z direction of the referenced coordinate system.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public ZDimensionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public ZDimensionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}