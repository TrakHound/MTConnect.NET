// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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