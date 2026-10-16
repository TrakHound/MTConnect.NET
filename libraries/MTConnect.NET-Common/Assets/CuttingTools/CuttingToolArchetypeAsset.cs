// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingToolArchetypeAsset
    {
        /// <summary>
        /// The fixed Asset type identifier ("CuttingToolArchetype") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public const string TypeId = "CuttingToolArchetype";


        /// <summary>
        /// Initializes a new CuttingToolArchetypeAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public CuttingToolArchetypeAsset()
        {
            Type = TypeId;
        }
    }
}
