// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// For those DataItem elements that report data that may be periodically reset to an initial value,
    /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
    /// </summary>
    public enum ResetTriggered
    {
        /// <summary>
        /// Not ResetTrigger was specified
        /// </summary>
        NOT_SPECIFIED,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a 12-month period.
        /// </summary>
        ACTION_COMPLETE,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a 24-hour period.
        /// </summary>
        ANNUAL,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a 24-hour period.
        /// </summary>
        DAY,

        /// <summary>
        /// The value of the Data Entity was reset upon completion of a maintenance event.
        /// </summary>
        MAINTENANCE,

        /// <summary>
        /// The value of the Data Entity was reset based on a physical reset action.
        /// </summary>
        MANUAL,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a monthly period.
        /// </summary>
        MONTH,

        /// <summary>
        /// The value of the Data Entity was reset when power was applied to the piece of 
        /// equipment after a planned or unplanned interruption of power has occurred.
        /// </summary>
        POWER_ON,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a work shift.
        /// </summary>
        SHIFT,

        /// <summary>
        /// The value of the Data Entity was reset at the end of a 7-day period
        /// </summary>
        WEEK
    }
}
