// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Dimension of an entity relative to the Y direction of the referenced coordinate system.
    /// </summary>
    public class YDimensionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "Y_DIMENSION";
        public const string NameId = "yDimension";
             
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Dimension of an entity relative to the Y direction of the referenced coordinate system.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public YDimensionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public YDimensionDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}