// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552384697_851849_3123

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that physically moves a tool from one location to another.
    /// </summary>
    public class TransferArmComposition : Composition 
    {
        public const string TypeId = "TRANSFER_ARM";
        public const string NameId = "transferArmComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that physically moves a tool from one location to another.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TransferArmComposition()  { Type = TypeId; }
    }
}