// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
    /// </summary>
    public class HydraulicComponent : Component 
    {
        public const string TypeId = "Hydraulic";
        public const string NameId = "hyd";

        public HydraulicComponent()  { Type = TypeId; }
    }
}
