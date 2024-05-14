// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemCategory
    {
        /// <summary>
        /// Information about the health of a piece of equipment and its ability to function.
        /// </summary>
        CONDITION,
        
        /// <summary>
        /// Discrete piece of information from the piece of equipment.
        /// </summary>
        EVENT,
        
        /// <summary>
        /// Continuously variable or analog data value. A continuous value can be measured at any point-in-time and will always produce a result.
        /// </summary>
        SAMPLE
    }
}