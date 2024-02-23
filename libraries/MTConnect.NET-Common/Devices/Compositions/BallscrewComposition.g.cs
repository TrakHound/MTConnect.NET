// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738861_555251_44678

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanical structure that transforms rotary motion into linear motion.
    /// </summary>
    public class BallscrewComposition : Composition 
    {
        public const string TypeId = "BALLSCREW";
        public const string NameId = "ballscrewComposition";
        public new const string DescriptionText = "Composition composed of a mechanical structure that transforms rotary motion into linear motion.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BallscrewComposition()  { Type = TypeId; }
    }
}