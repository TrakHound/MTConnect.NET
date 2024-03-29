// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Force relative to earth�s gravity.
    /// </summary>
    public class GravitationalForceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "GRAVITATIONAL_FORCE";
        public const string NameId = "gravForce";
        public const string DefaultNativeUnits = Devices.NativeUnits.GRAVITATIONAL_FORCE;
        public new const string DescriptionText = "Force relative to earth�s gravity.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version21;


        public GravitationalForceDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            NativeUnits = DefaultNativeUnits;
        }

        public GravitationalForceDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            NativeUnits = DefaultNativeUnits;
        }
    }
}