// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanical structure that transforms rotary motion into linear motion.
    /// </summary>
    public class BallscrewCompositionComposition : Composition 
    {
        public const string TypeId = "BALLSCREW";
        public const string NameId = "ballscrewComposition";
        public new const string DescriptionText = "Composition composed of a mechanical structure that transforms rotary motion into linear motion.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BallscrewCompositionComposition()  { Type = TypeId; }
    }
}