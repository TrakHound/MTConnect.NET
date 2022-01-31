// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// A Linear axis represents prismatic motion along a fixed axis.
    /// </summary>
    public class LinearComponent : Component 
    {
        public const string TypeId = "Linear";
        public const string NameId = "lin";

        public LinearComponent()  { Type = TypeId; }
    }
}
