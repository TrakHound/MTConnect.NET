// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738883_904706_44744

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that provides or applies a stretch or strain to another mechanism.
    /// </summary>
    public class TensionerComposition : Composition 
    {
        public const string TypeId = "TENSIONER";
        public const string NameId = "tensionerComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that provides or applies a stretch or strain to another mechanism.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public TensionerComposition()  { Type = TypeId; }
    }
}