// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106468_61130_44414

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that produces a current of air.
    /// </summary>
    public class FanComponent : Component
    {
        public const string TypeId = "Fan";
        public const string NameId = "fan";
        public new const string DescriptionText = "Leaf Component that produces a current of air.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public FanComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}