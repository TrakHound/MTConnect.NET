// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Operator’s overridden value.
    /// </summary>
    public class PathFeedrateOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PATH_FEEDRATE_OVERRIDE";
        public const string NameId = "pathFeedrateOverride";
        public new const string DescriptionText = "Operator’s overridden value.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MaximumVersion => MTConnectVersions.Version12;


        public PathFeedrateOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public PathFeedrateOverrideDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
