// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanical structure for transforming rotary motion into linear motion.
    /// </summary>
    public class BallscrewComposition : Composition 
    {
        public const string TypeId = "BALLSCREW";
        public const string NameId = "bscrew";

        public BallscrewComposition()  { Type = TypeId; }
    }
}
