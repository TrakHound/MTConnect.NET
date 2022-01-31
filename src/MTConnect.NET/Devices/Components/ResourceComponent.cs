// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resource is an abstract Component that represents materials or personnel involved in a manufacturing process.
    /// </summary>
    public class ResourceComponent : Component 
    {
        public const string TypeId = "Resource";
        public const string NameId = "resource";

        public ResourceComponent()  { Type = TypeId; }
    }
}
