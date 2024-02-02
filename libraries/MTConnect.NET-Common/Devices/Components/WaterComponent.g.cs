// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106480_337779_44480

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of $$H_2 O$$.
    /// </summary>
    public class WaterComponent : Component
    {
        public const string TypeId = "Water";
        public const string NameId = "water";
        public new const string DescriptionText = "Leaf Component composed of $$H_2 O$$.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WaterComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}