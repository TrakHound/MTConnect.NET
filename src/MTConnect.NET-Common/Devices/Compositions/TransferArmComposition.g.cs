// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that physically moves a tool from one location to another.
    /// </summary>
    public class TransferArmCompositionComposition : Composition 
    {
        public const string TypeId = "TRANSFER_ARM";
        public const string NameId = "transferArmComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that physically moves a tool from one location to another.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TransferArmCompositionComposition()  { Type = TypeId; }
    }
}