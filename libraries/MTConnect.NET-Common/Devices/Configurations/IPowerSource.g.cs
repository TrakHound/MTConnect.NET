// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Potential energy sources for the Component.
    /// </summary>
    public interface IPowerSource
    {
        /// <summary>
        /// Reference to the Component providing observations about the power source.
        /// </summary>
        string ComponentIdRef { get; }
        
        /// <summary>
        /// Unique identifier for the power source.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Optional precedence for a given power source.
        /// </summary>
        int Order { get; }
        
        /// <summary>
        /// Type of the power source.
        /// </summary>
        MTConnect.Devices.Configurations.PowerSourceType Type { get; }
        
        /// <summary>
        /// Name of the power source.
        /// </summary>
        string Value { get; }
    }
}