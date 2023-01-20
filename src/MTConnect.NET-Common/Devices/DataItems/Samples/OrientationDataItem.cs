// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// A measured or calculated orientation of a plane or vector relative to a cartesian coordinate system
    /// </summary>
    public class OrientationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ORIENTATION";
        public const string NameId = "orientation";
        public const string DefaultUnits = Devices.Units.DEGREE_3D;
        public new const string DescriptionText = "A measured or calculated orientation of a plane or vector relative to a cartesian coordinate system";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public OrientationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public OrientationDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}