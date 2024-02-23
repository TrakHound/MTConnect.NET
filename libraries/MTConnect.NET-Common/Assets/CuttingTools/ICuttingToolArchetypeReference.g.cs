// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// AssetId and/or the URL of the data source of CuttingToolArchetype.
    /// </summary>
    public interface ICuttingToolArchetypeReference
    {
        /// <summary>
        /// URL of the CuttingToolArchetype information model.
        /// </summary>
        string Source { get; }
        
        /// <summary>
        /// `assetId` of the related CuttingToolArchetype.
        /// </summary>
        string Value { get; }
    }
}