// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_D4CAAB4A_DE00_489d_ACCA_F00FC7296F0C

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Reference information about the assetId and/or the URL of the data source of CuttingToolArchetype.
    /// </summary>
    public class CuttingToolArchetypeReference : ICuttingToolArchetypeReference
    {
        public const string DescriptionText = "Reference information about the assetId and/or the URL of the data source of CuttingToolArchetype.";


        /// <summary>
        /// Url of the CuttingToolArchetype information model.
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// Assetid of the related CuttingToolArchetype.
        /// </summary>
        public string Value { get; set; }
    }
}