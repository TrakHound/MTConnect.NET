// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Method used to compute standard uncertainty.
    /// </summary>
    public enum UncertaintyType
    {
        /// <summary>
        /// Combined standard uncertainty.
        /// </summary>
        COMBINED,
        
        /// <summary>
        /// Standard uncertainty using arithmetic mean or average the observations. JCGM 100:2008 4.2
        /// </summary>
        MEAN
    }
}