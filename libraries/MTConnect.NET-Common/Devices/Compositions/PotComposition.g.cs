// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552375625_673957_3119

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.
    /// </summary>
    public class PotComposition : Composition 
    {
        public const string TypeId = "POT";
        public const string NameId = "potComposition";
        public new const string DescriptionText = "Composition composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public PotComposition()  { Type = TypeId; }
    }
}