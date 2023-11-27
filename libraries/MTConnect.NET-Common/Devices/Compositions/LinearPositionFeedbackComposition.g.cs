// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that measures linear motion or position.
    /// </summary>
    public class LinearPositionFeedbackCompositionComposition : Composition 
    {
        public const string TypeId = "LINEAR_POSITION_FEEDBACK";
        public const string NameId = "linearPositionFeedbackComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that measures linear motion or position.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public LinearPositionFeedbackCompositionComposition()  { Type = TypeId; }
    }
}