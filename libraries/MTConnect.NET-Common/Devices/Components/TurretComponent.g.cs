// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605551876111_523604_2450

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.
    /// </summary>
    public class TurretComponent : Component
    {
        public const string TypeId = "Turret";
        public const string NameId = "turret";
        public new const string DescriptionText = "ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TurretComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}