// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Absolute value of the change in angular position around a vector
    /// </summary>
    public class DisplacementAngularDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DISPLACEMENT_ANGULAR";
        public const string NameId = "dispAngular";
        public const string DefaultUnits = Devices.Units.DEGREE;
        public new const string DescriptionText = "Absolute value of the change in angular position around a vector";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version21;


        public DisplacementAngularDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DisplacementAngularDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = 3;
        }
    }
}