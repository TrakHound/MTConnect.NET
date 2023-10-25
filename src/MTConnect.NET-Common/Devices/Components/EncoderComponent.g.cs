// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106466_630446_44405

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that measures position.
    /// </summary>
    public class EncoderComponent : Component
    {
        public const string TypeId = "Encoder";
        public const string NameId = "encoderComponent";
        public new const string DescriptionText = "Leaf Component that measures position.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public EncoderComponent() { Type = TypeId; }
    }
}