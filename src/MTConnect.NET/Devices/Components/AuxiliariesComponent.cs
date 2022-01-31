// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliaries organizes Auxiliary component types.
    /// </summary>
    public class AuxiliariesComponent : Component 
    {
        public const string TypeId = "Auxiliaries";
        public const string NameId = "aux";

        public AuxiliariesComponent()  { Type = TypeId; }
    }
}
