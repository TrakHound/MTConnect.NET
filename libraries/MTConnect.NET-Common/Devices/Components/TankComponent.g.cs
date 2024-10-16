// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106477_651135_44465

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component generally composed of an enclosed container.
    /// </summary>
    public class TankComponent : Component
    {
        public const string TypeId = "Tank";
        public const string NameId = "tank";
        public new const string DescriptionText = "Leaf Component generally composed of an enclosed container.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public TankComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}