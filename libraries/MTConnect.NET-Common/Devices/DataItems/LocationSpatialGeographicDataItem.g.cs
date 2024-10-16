// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1712322699365_211671_599

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Absolute geographic location defined by two coordinates, longitude and latitude and an elevation.
    /// </summary>
    public class LocationSpatialGeographicDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LOCATION_SPATIAL_GEOGRAPHIC";
        public const string NameId = "locationSpatialGeographic";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.TABLE;     
             
        public new const string DescriptionText = "Absolute geographic location defined by two coordinates, longitude and latitude and an elevation.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version23;       


        public LocationSpatialGeographicDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public LocationSpatialGeographicDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}