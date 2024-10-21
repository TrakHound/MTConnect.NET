// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationSpatialGeographicResult : EventDataSetObservation
    {

        /// <summary>
        /// Height relative to a reference.
        /// </summary>
        public string Altitude 
        { 
            get => GetValue<string>("DataSet[Altitude]");
            set => AddValue("DataSet[Altitude]", value);
        }
        
        /// <summary>
        /// Geographic latitude.
        /// </summary>
        public string Latitude 
        { 
            get => GetValue<string>("DataSet[Latitude]");
            set => AddValue("DataSet[Latitude]", value);
        }
        
        /// <summary>
        /// Geographic longitude.
        /// </summary>
        public string Longitude 
        { 
            get => GetValue<string>("DataSet[Longitude]");
            set => AddValue("DataSet[Longitude]", value);
        }
    }
}