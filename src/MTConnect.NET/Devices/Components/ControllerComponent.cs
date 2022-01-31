// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Controller represents the computational regulation and management function of a piece of equipment.
    /// </summary>
    public class ControllerComponent : Component 
    {
        public const string TypeId = "Controller";
        public const string NameId = "cont";

        public ControllerComponent()  { Type = TypeId; }
    }
}
