// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The value of a command issued to adjust the programmed velocity for a Rotary type axis.
    /// </summary>
    public class RotaryVelocityOverrideValue : EventValue
    {
        public RotaryVelocityOverrideValue(double percent)
        {
            Value = percent;
        }
    }
}