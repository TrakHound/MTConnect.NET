// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanical structure for transforming rotary motion into linear motion.
    /// </summary>
    public class BallscrewComposition : Composition 
    {
        public const string TypeId = "BALLSCREW";
        public const string NameId = "bscrew";
        public new const string DescriptionText = "A mechanical structure for transforming rotary motion into linear motion.";

        public override string TypeDescription => DescriptionText;


        public BallscrewComposition()  { Type = TypeId; }
    }
}