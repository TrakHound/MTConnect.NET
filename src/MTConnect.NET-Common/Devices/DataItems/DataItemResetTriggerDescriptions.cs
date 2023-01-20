// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemResetTriggerDescriptions
    {
        /// <summary>
        /// Reset Trigger Not Set
        /// </summary>
        public const string NONE = "Reset Trigger Not Set";

        /// <summary>
        /// The value of the Data Entity that is measuring an action or operation is to be reset upon completion of that action or operation.
        /// </summary>
        public const string ACTION_COMPLETE = "The value of the Data Entity that is measuring an action or operation is to be reset upon completion of that action or operation.";

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 12-month period.
        /// </summary>
        public const string ANNUAL = "The value of the Data Entity is to be reset at the end of a 12-month period.";

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 24-hour period.
        /// </summary>
        public const string DAY = "The value of the Data Entity is to be reset at the end of a 24-hour period.";

        /// <summary>
        /// The value of the Data Entity is not reset and accumulates for the entire life of the piece of equipment.
        /// </summary>
        public const string LIFE = "The value of the Data Entity is not reset and accumulates for the entire life of the piece of equipment.";

        /// <summary>
        /// The value of the Data Entity is to be reset upon completion of a maintenance event.
        /// </summary>
        public const string MAINTENANCE = "The value of the Data Entity is to be reset upon completion of a maintenance event.";

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a monthly period.
        /// </summary>
        public const string MONTH = "The value of the Data Entity is to be reset at the end of a monthly period.";

        /// <summary>
        /// The value of the Data Entity is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.
        /// </summary>
        public const string POWER_ON = "The value of the Data Entity is to be reset when power was applied to the piece of equipment after a planned or unplanned interruption of power has occurred.";

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a work shift.
        /// </summary>
        public const string SHIFT = "The value of the Data Entity is to be reset at the end of a work shift.";

        /// <summary>
        /// The value of the Data Entity is to be reset at the end of a 7-day period.
        /// </summary>
        public const string WEEK = "The value of the Data Entity is to be reset at the end of a 7-day period.";


        public static string Get(DataItemResetTrigger dataItemResetTrigger)
        {
            switch (dataItemResetTrigger)
            {
                case DataItemResetTrigger.NONE: return NONE;
                case DataItemResetTrigger.ACTION_COMPLETE: return ACTION_COMPLETE;
                case DataItemResetTrigger.ANNUAL: return ANNUAL;
                case DataItemResetTrigger.DAY: return DAY;
                case DataItemResetTrigger.LIFE: return LIFE;
                case DataItemResetTrigger.MAINTENANCE: return MAINTENANCE;
                case DataItemResetTrigger.MONTH: return MONTH;
                case DataItemResetTrigger.POWER_ON: return POWER_ON;
                case DataItemResetTrigger.SHIFT: return SHIFT;
                case DataItemResetTrigger.WEEK: return WEEK;
            }

            return "";
        }
    }
}