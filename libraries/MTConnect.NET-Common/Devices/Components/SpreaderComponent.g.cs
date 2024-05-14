// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106475_909781_44453

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that flattens or spreading materials.
    /// </summary>
    public class SpreaderComponent : Component
    {
        public const string TypeId = "Spreader";
        public const string NameId = "spreader";
        public new const string DescriptionText = "Leaf Component that flattens or spreading materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public SpreaderComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}