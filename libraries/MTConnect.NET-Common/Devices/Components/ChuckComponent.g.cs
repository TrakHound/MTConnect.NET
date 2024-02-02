// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_4_45f01b9_1643679566213_508045_1804

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a mechanism that holds a part or stock material in place.
    /// </summary>
    public class ChuckComponent : Component
    {
        public const string TypeId = "Chuck";
        public const string NameId = "chuck";
        public new const string DescriptionText = "Leaf Component composed of a mechanism that holds a part or stock material in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13; 


        public ChuckComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}