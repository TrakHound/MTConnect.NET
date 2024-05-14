// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106472_287785_44435

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a viscous liquid.
    /// </summary>
    public class OilComponent : Component
    {
        public const string TypeId = "Oil";
        public const string NameId = "oil";
        public new const string DescriptionText = "Leaf Component composed of a viscous liquid.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public OilComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}