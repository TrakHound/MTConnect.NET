// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resources organizes Resource component types.
    /// </summary>
    public class ResourcesComponent : Component 
    {
        public const string TypeId = "Resources";
        public const string NameId = "resources";

        public ResourcesComponent()  { Type = TypeId; }
    }
}
