// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738870_246045_44702

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that measures rotary position.
    /// </summary>
    public class EncoderComposition : Composition 
    {
        public const string TypeId = "ENCODER";
        public const string NameId = "encoderComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that measures rotary position.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public EncoderComposition()  { Type = TypeId; }
    }
}