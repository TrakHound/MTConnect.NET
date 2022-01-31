// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Systems organizes System component types
    /// </summary>
    public class SystemsComponent : Component 
    {
        public const string TypeId = "Systems";
        public const string NameId = "sys";

        public SystemsComponent()  { Type = TypeId; }
    }
}
