// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Angular displacement.
    /// </summary>
    public class TiltDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "TILT";
        public const string NameId = "tilt";
        public const string DefaultUnits = Devices.Units.MICRO_RADIAN;     
        public new const string DescriptionText = "Angular displacement.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public TiltDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public TiltDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}