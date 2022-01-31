// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Structure is a Component that represents the part(s) comprising the rigid bodies of the piece of equipment.
    /// </summary>
    public class StructureComponent : Component 
    {
        public const string TypeId = "Structure";
        public const string NameId = "struct";

        public StructureComponent()  { Type = TypeId; }
    }
}
