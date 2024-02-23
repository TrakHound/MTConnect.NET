// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1587597358521_716746_1028

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of part(s) comprising the rigid bodies of the piece of equipment.
    /// </summary>
    public class StructureComponent : Component
    {
        public const string TypeId = "Structure";
        public const string NameId = "structure";
        public new const string DescriptionText = "Component composed of part(s) comprising the rigid bodies of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public StructureComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}