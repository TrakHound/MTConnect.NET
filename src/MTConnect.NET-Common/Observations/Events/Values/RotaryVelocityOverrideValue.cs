// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
