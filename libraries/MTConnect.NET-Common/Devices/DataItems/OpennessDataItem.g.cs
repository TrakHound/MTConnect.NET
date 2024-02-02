// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Percentage open where 100% is fully open and 0% is fully closed.
    /// </summary>
    public class OpennessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "OPENNESS";
        public const string NameId = "openness";
             
        public const string DefaultUnits = Devices.Units.PERCENT;     
        public new const string DescriptionText = "Percentage open where 100% is fully open and 0% is fully closed.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version20;       


        public OpennessDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public OpennessDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}