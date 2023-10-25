// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605551853978_27109_2354

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Toolingdelivery composed of a tool delivery mechanism that moves tools between a ToolMagazine and a spindle a Turret.
    /// </summary>
    public class AutomaticToolChangerComponent : Component
    {
        public const string TypeId = "AutomaticToolChanger";
        public const string NameId = "automaticToolChangerComponent";
        public new const string DescriptionText = "Toolingdelivery composed of a tool delivery mechanism that moves tools between a ToolMagazine and a spindle a Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public AutomaticToolChangerComponent() { Type = TypeId; }
    }
}