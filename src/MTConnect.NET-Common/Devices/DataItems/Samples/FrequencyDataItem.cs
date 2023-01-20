// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the number of occurrences of a repeating event per unit time.
    /// </summary>
    public class FrequencyDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "FREQUENCY";
        public const string NameId = "frequency";
        public const string DefaultUnits = Devices.Units.HERTZ;
        public new const string DescriptionText = "The measurement of the number of occurrences of a repeating event per unit time.";

        public override string TypeDescription => DescriptionText;


        public FrequencyDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public FrequencyDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}