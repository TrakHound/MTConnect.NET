// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_D4CAAB4A_DE00_489d_ACCA_F00FC7296F0C

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// AssetId and/or the URL of the data source of CuttingToolArchetype.
    /// </summary>
    public class CuttingToolArchetypeReference : ICuttingToolArchetypeReference
    {
        public const string DescriptionText = "AssetId and/or the URL of the data source of CuttingToolArchetype.";


        /// <summary>
        /// URL of the CuttingToolArchetype information model.
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// `assetId` of the related CuttingToolArchetype.
        /// </summary>
        public string Value { get; set; }
    }
}