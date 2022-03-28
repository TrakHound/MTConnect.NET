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
        public new const string DescriptionText = "Resource is an abstract Component that represents materials or personnel involved in a manufacturing process.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public ResourceComponent()  { Type = TypeId; }
    }
}
