// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Pressure is a System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
    /// </summary>
    public class PressureComponent : Component 
    {
        public const string TypeId = "Pressure";
        public const string NameId = "press";

        public PressureComponent()  { Type = TypeId; }
    }
}
