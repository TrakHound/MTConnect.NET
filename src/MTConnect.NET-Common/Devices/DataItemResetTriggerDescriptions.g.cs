// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemResetTriggerDescriptions
    {
        /// <summary>
        /// Observation of the DataItem that is measuring an action or operation is to be reset upon completion of that action or operation.
        /// </summary>
        public const string ACTION_COMPLETE = "Observation of the DataItem that is measuring an action or operation is to be reset upon completion of that action or operation.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 12-month period.
        /// </summary>
        public const string ANNUAL = "Observation of the DataItem is to be reset at the end of a 12-month period.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 24-hour period.
        /// </summary>
        public const string DAY = "Observation of the DataItem is to be reset at the end of a 24-hour period.";
        
        /// <summary>
        /// Observation of the DataItem is not reset and accumulates for the entire life of the piece of equipment.
        /// </summary>
        public const string LIFE = "Observation of the DataItem is not reset and accumulates for the entire life of the piece of equipment.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset upon completion of a maintenance event.
        /// </summary>
        public const string MAINTENANCE = "Observation of the DataItem is to be reset upon completion of a maintenance event.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a monthly period.
        /// </summary>
        public const string MONTH = "Observation of the DataItem is to be reset at the end of a monthly period.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.
        /// </summary>
        public const string POWER_ON = "Observation of the DataItem is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a work shift.
        /// </summary>
        public const string SHIFT = "Observation of the DataItem is to be reset at the end of a work shift.";
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 7-day period.
        /// </summary>
        public const string WEEK = "Observation of the DataItem is to be reset at the end of a 7-day period.";


        public static string Get(DataItemResetTrigger value)
        {
            switch (value)
            {
                case DataItemResetTrigger.ACTION_COMPLETE: return "Observation of the DataItem that is measuring an action or operation is to be reset upon completion of that action or operation.";
                case DataItemResetTrigger.ANNUAL: return "Observation of the DataItem is to be reset at the end of a 12-month period.";
                case DataItemResetTrigger.DAY: return "Observation of the DataItem is to be reset at the end of a 24-hour period.";
                case DataItemResetTrigger.LIFE: return "Observation of the DataItem is not reset and accumulates for the entire life of the piece of equipment.";
                case DataItemResetTrigger.MAINTENANCE: return "Observation of the DataItem is to be reset upon completion of a maintenance event.";
                case DataItemResetTrigger.MONTH: return "Observation of the DataItem is to be reset at the end of a monthly period.";
                case DataItemResetTrigger.POWER_ON: return "Observation of the DataItem is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.";
                case DataItemResetTrigger.SHIFT: return "Observation of the DataItem is to be reset at the end of a work shift.";
                case DataItemResetTrigger.WEEK: return "Observation of the DataItem is to be reset at the end of a 7-day period.";
            }

            return null;
        }
    }
}