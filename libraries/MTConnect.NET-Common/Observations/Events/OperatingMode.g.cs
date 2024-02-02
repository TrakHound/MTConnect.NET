// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of Component or Composition that describes the automatic or manual operation of the entity.
    /// </summary>
    public enum OperatingMode
    {
        /// <summary>
        /// Automatically execute instructions from a recipe or program.> Note: Setpoint comes from a recipe.
        /// </summary>
        AUTOMATIC,
        
        /// <summary>
        /// Execute instructions from an external agent or person.> Note 1 to entry: Valve or switch is manipulated by an agent/person.> Note 2 to entry: Direct control of the PID output. % of the range: A user manually sets the % output, not the setpoint.
        /// </summary>
        MANUAL,
        
        /// <summary>
        /// Executes a single instruction from a recipe or program.> Note 1 to entry: Setpoint is entered and fixed, but the PID is controlling.> Note 2 to entry: Still goes through the PID control system.> Note 3 to entry: Manual fixed entry from a recipe.
        /// </summary>
        SEMI_AUTOMATIC
    }
}