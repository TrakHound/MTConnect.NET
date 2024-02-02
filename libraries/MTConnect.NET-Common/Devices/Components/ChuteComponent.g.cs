// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106463_904140_44387

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of an inclined channel that conveys material.
    /// </summary>
    public class ChuteComponent : Component
    {
        public const string TypeId = "Chute";
        public const string NameId = "chute";
        public new const string DescriptionText = "Leaf Component composed of an inclined channel that conveys material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChuteComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}