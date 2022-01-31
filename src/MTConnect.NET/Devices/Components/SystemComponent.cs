// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System is an abstract Component that represents part(s) of a piece of equipment that is permanently integrated into the piece of equipment.
    /// </summary>
    public class SystemComponent : Component 
    {
        public const string TypeId = "System";
        public const string NameId = "sys";

        public SystemComponent()  { Type = TypeId; }
    }
}
