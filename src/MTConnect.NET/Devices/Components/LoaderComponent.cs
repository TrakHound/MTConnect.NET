// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Loader is an Auxiliary comprised of all the parts involved in moving and distributing materials, parts, tooling, and other items to or from a piece of equipment.
    /// </summary>
    public class LoaderComponent : Component 
    {
        public const string TypeId = "Loader";
        public const string NameId = "load";

        public LoaderComponent()  { Type = TypeId; }
    }
}
