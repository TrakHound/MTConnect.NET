// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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