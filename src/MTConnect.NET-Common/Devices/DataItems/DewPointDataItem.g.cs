// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.
    /// </summary>
    public class DewPointDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DEW_POINT";
        public const string NameId = "";
        public const string DefaultUnits = Devices.Units.CELSIUS;     
        public new const string DescriptionText = "Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public DewPointDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DewPointDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}