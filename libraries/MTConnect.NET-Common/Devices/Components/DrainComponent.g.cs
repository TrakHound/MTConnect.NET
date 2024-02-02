// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106466_143410_44402

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
    /// </summary>
    public class DrainComponent : Component
    {
        public const string TypeId = "Drain";
        public const string NameId = "drain";
        public new const string DescriptionText = "Leaf Component that allows material to flow for the purpose of drainage from, for example, a vessel or tank.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public DrainComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}