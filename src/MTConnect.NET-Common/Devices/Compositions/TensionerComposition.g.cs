// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that provides or applies a stretch or strain to another mechanism.
    /// </summary>
    public class TensionerCompositionComposition : Composition 
    {
        public const string TypeId = "TENSIONER";
        public const string NameId = "tensionerComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that provides or applies a stretch or strain to another mechanism.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public TensionerCompositionComposition()  { Type = TypeId; }
    }
}