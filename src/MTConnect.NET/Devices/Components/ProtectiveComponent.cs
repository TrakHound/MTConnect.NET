// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Protective is a System that represents the information for those functions that detect or prevent harm or damage to equipment or personnel. 
    /// Protective does not include the information relating to the Enclosure system.
    /// </summary>
    public class ProtectiveComponent : Component 
    {
        public const string TypeId = "Protective";
        public const string NameId = "protect";

        public ProtectiveComponent()  { Type = TypeId; }
    }
}
