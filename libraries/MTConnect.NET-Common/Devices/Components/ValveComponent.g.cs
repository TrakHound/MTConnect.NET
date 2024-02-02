// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106478_602921_44474

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that halts or controls the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.
    /// </summary>
    public class ValveComponent : Component
    {
        public const string TypeId = "Valve";
        public const string NameId = "valve";
        public new const string DescriptionText = "Leaf Component that halts or controls the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ValveComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}