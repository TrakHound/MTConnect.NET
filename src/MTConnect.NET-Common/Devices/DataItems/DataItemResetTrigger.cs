// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
    /// It is additional information regarding the meaning of the data that establishes an understanding of the
    /// time frame that the data represents so that the data may be correctly understood by a client software application.
    /// </summary>
    public enum DataItemResetTrigger
    {
        /// <summary>
        /// Reset Trigger Not Set
        /// </summary>
        NONE,

        /// <summary>
        /// The value of the Data Entity that is measuring an action or operation is to be reset upon completion of that action or operation.
        /// </summary>
        ACTION_COMPLETE,

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 12-month period.
        /// </summary>
        ANNUAL,

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 24-hour period.
        /// </summary>
        DAY,

        /// <summary>
        /// The value of the Data Entity is not reset and accumulates for the entire life of the piece of equipment.
        /// </summary>
        LIFE,

        /// <summary>
        /// The value of the Data Entity is to be reset upon completion of a maintenance event.
        /// </summary>
        MAINTENANCE,

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a monthly period.
        /// </summary>
        MONTH,

        /// <summary>
        /// The value of the Data Entity is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.
        /// </summary>
        POWER_ON,

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a work shift.
        /// </summary>
        SHIFT,

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 7-day period.
        /// </summary>
        WEEK
    }
}