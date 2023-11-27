// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381987_89386_42234

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of a structure that is used to contain or isolate a piece of equipment or area.
    /// </summary>
    public class EnclosureComponent : Component
    {
        public const string TypeId = "Enclosure";
        public const string NameId = "enclosureComponent";
        public new const string DescriptionText = "System composed of a structure that is used to contain or isolate a piece of equipment or area.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public EnclosureComponent() { Type = TypeId; }
    }
}