// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// AutomaticToolChanger is a ToolingDelivery that represents a tool delivery mechanism that moves tools between a ToolMagazine and a Spindle or a Turret.
    /// An AutomaticToolChanger may also transfer tools between a location outside of a piece of equipment and a ToolMagazine or Turret.
    /// </summary>
    public class AutomaticToolChangerComponent : Component 
    {
        public const string TypeId = "AutomaticToolChanger";
        public const string NameId = "atc";

        public AutomaticToolChangerComponent()  { Type = TypeId; }
    }
}
