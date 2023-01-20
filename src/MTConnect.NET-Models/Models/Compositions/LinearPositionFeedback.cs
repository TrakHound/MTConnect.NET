// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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