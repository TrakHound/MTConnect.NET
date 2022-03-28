// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Structures organizes Structure component types.
    /// </summary>
    public class StructuresComponent : Component 
    {
        public const string TypeId = "Structures";
        public const string NameId = "structs";
        public new const string DescriptionText = "Structures organizes Structure component types.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public StructuresComponent()  { Type = TypeId; }
    }
}
