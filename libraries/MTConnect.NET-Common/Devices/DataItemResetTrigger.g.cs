// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemResetTrigger
    {
        /// <summary>
        /// Observation of the DataItem that is measuring an action or operation is to be reset upon completion of that action or operation.
        /// </summary>
        ACTION_COMPLETE,
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 12-month period.
        /// </summary>
        ANNUAL,
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 24-hour period.
        /// </summary>
        DAY,
        
        /// <summary>
        /// Observation of the DataItem is not reset and accumulates for the entire life of the piece of equipment.
        /// </summary>
        LIFE,
        
        /// <summary>
        /// Observation of the DataItem is to be reset upon completion of a maintenance event.
        /// </summary>
        MAINTENANCE,
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a monthly period.
        /// </summary>
        MONTH,
        
        /// <summary>
        /// Observation of the DataItem is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.
        /// </summary>
        POWER_ON,
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a work shift.
        /// </summary>
        SHIFT,
        
        /// <summary>
        /// Observation of the DataItem is to be reset at the end of a 7-day period.
        /// </summary>
        WEEK
    }
}