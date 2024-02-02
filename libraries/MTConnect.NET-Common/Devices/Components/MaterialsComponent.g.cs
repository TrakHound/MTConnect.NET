// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1630059859468_228796_88

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resources that organize Material types.
    /// </summary>
    public class MaterialsComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Materials";
        public const string NameId = "materials";
        public new const string DescriptionText = "Resources that organize Material types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public MaterialsComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}