// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738874_426745_44718

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that measures linear motion or position.
    /// </summary>
    public class LinearPositionFeedbackComposition : Composition 
    {
        public const string TypeId = "LINEAR_POSITION_FEEDBACK";
        public const string NameId = "linearPositionFeedbackComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that measures linear motion or position.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public LinearPositionFeedbackComposition()  { Type = TypeId; }
    }
}