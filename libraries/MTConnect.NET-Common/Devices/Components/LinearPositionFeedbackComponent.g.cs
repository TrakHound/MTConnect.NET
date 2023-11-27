// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106471_40319_44429

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that measures linear motion or position.**DEPRECATION WARNING** : May be deprecated in the future. Recommend using Encoder.
    /// </summary>
    public class LinearPositionFeedbackComponent : Component
    {
        public const string TypeId = "LinearPositionFeedback";
        public const string NameId = "linearPositionFeedbackComponent";
        public new const string DescriptionText = "Leaf Component that measures linear motion or position.**DEPRECATION WARNING** : May be deprecated in the future. Recommend using Encoder.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public LinearPositionFeedbackComponent() { Type = TypeId; }
    }
}