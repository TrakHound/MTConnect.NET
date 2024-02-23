// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class SensorAttachmentResult : EventDataSetObservation
    {

        /// <summary>
        /// Identity of a sensor used to observe some measurement of an item.
        /// </summary>
        public string SensorId 
        { 
            get => GetValue<string>("DataSet[SensorId]");
            set => AddValue("DataSet[SensorId]", value);
        }
    }
}