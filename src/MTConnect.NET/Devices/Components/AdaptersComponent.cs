// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Adapters organizes Adapter component types.
    /// </summary>
    public class AdaptersComponent : Component 
    {
        public const string TypeId = "Adapters";
        public const string NameId = "adapters";

        public AdaptersComponent()  { Type = TypeId; }
    }
}
