// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that measures linear motion or position.
    /// </summary>
    public class LinearPositionFeedbackModel : CompositionModel, ILinearPositionFeedbackModel
    {
        public LinearPositionFeedbackModel() 
        {
            Type = LinearPositionFeedbackComposition.TypeId;
        }

        public LinearPositionFeedbackModel(string compositionId)
        {
            Id = compositionId;
            Type = LinearPositionFeedbackComposition.TypeId;
        }
    }
}
