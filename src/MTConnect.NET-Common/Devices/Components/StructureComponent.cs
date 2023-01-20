// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Structure is a Component that represents the part(s) comprising the rigid bodies of the piece of equipment.
    /// </summary>
    public class StructureComponent : Component 
    {
        public const string TypeId = "Structure";
        public const string NameId = "struct";
        public new const string DescriptionText = "Structure is a Component that represents the part(s) comprising the rigid bodies of the piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public StructureComponent()  { Type = TypeId; }
    }
}