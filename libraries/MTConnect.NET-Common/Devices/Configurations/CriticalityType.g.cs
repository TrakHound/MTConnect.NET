// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum CriticalityType
    {
        /// <summary>
        /// Services or functions provided by the associated element is required for the operation of this element.
        /// </summary>
        CRITICAL,
        
        /// <summary>
        /// Services or functions provided by the associated element is not required for the operation of this element.
        /// </summary>
        NONCRITICAL
    }
}