// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measured dimension of a diameter.
    /// </summary>
    public class DiameterDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DIAMETER";
        public const string NameId = "dia";
        public const string DefaultUnits = Devices.Units.MILLIMETER;
        public new const string DescriptionText = "The measured dimension of a diameter.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public DiameterDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DiameterDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}