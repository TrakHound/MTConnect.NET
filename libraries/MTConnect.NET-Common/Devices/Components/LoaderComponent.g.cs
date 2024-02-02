// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381999_206845_42258

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that provides movement and distribution of materials, parts, tooling, and other items to or from a piece of equipment.
    /// </summary>
    public class LoaderComponent : Component
    {
        public const string TypeId = "Loader";
        public const string NameId = "loader";
        public new const string DescriptionText = "Auxiliary that provides movement and distribution of materials, parts, tooling, and other items to or from a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public LoaderComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}