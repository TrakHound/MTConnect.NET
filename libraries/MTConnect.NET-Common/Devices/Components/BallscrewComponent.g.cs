// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106460_88955_44369

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a mechanical structure that transforms rotary motion into linear motion.
    /// </summary>
    public class BallscrewComponent : Component
    {
        public const string TypeId = "Ballscrew";
        public const string NameId = "ballscrew";
        public new const string DescriptionText = "Leaf Component composed of a mechanical structure that transforms rotary motion into linear motion.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BallscrewComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}