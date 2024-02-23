// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106473_448314_44444

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.
    /// </summary>
    public class PumpComponent : Component
    {
        public const string TypeId = "Pump";
        public const string NameId = "pump";
        public new const string DescriptionText = "Leaf Component that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PumpComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}