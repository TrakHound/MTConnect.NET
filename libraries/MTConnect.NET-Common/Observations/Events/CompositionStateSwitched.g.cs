// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public enum CompositionStateSwitched
    {
        /// <summary>
        /// Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.
        /// </summary>
        ON,
        
        /// <summary>
        /// Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.
        /// </summary>
        OFF
    }
}