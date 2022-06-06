// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanical structure for transforming rotary motion into linear motion.
    /// </summary>
    public class BallscrewModel : CompositionModel, IBallscrewModel
    {
        public BallscrewModel() 
        {
            Type = BallscrewComposition.TypeId;
        }

        public BallscrewModel(string compositionId)
        {
            Id = compositionId;
            Type = BallscrewComposition.TypeId;
        }
    }
}
