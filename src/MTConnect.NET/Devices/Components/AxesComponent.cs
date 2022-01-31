// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axes organizes Axis component types.
    /// </summary>
    public class AxesComponent : Component 
    {
        public const string TypeId = "Axes";
        public const string NameId = "axes";

        public AxesComponent()  { Type = TypeId; }
    }
}
