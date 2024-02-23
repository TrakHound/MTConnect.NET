// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738867_238330_44694

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that strengthens, supports, or fastens objects in place.
    /// </summary>
    public class ClampComposition : Composition 
    {
        public const string TypeId = "CLAMP";
        public const string NameId = "clampComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that strengthens, supports, or fastens objects in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ClampComposition()  { Type = TypeId; }
    }
}