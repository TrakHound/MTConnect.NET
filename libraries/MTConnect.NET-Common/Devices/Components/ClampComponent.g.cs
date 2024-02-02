// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106464_145351_44393

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that strengthens, support, or fastens objects in place.
    /// </summary>
    public class ClampComponent : Component
    {
        public const string TypeId = "Clamp";
        public const string NameId = "clamp";
        public new const string DescriptionText = "Leaf Component that strengthens, support, or fastens objects in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ClampComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}