// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that measures rotary position.
    /// </summary>
    public class EncoderCompositionComposition : Composition 
    {
        public const string TypeId = "ENCODER";
        public const string NameId = "encoderComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that measures rotary position.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public EncoderCompositionComposition()  { Type = TypeId; }
    }
}