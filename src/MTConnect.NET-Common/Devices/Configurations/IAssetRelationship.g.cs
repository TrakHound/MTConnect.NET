// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configurationrelationship that describes the association between a Component and an Asset.
    /// </summary>
    public interface IAssetRelationship : IRelationship
    {
        /// <summary>
        /// Uuid of the related Asset.
        /// </summary>
        string AssetIdRef { get; }
        
        /// <summary>
        /// Type of Asset being referenced.
        /// </summary>
        string AssetType { get; }
        
        /// <summary>
        /// Uri reference to the associated Asset.
        /// </summary>
        string Href { get; }
    }
}