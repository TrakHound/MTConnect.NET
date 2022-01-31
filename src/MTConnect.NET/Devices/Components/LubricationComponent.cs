// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.
    /// </summary>
    public class LubricationComponent : Component 
    {
        public const string TypeId = "Lubrication";
        public const string NameId = "lube";

        public LubricationComponent()  { Type = TypeId; }
    }
}
