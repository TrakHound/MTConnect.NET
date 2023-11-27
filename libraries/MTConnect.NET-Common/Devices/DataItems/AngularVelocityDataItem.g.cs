// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Rate of change of angular position.
    /// </summary>
    public class AngularVelocityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ANGULAR_VELOCITY";
        public const string NameId = "angularVelocity";
        public const string DefaultUnits = Devices.Units.DEGREE_PER_SECOND;     
        public new const string DescriptionText = "Rate of change of angular position.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public AngularVelocityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public AngularVelocityDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}