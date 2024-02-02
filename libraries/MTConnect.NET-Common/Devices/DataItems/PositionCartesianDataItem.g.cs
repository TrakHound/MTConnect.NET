// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Point in a cartesian coordinate system.
    /// </summary>
    public class PositionCartesianDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "POSITION_CARTESIAN";
        public const string NameId = "positionCartesian";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
        public const string DefaultUnits = Devices.Units.MILLIMETER_3D;     
        public new const string DescriptionText = "Point in a cartesian coordinate system.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version21;       


        public PositionCartesianDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            Units = DefaultUnits;
        }

        public PositionCartesianDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            Units = DefaultUnits;
        }
    }
}