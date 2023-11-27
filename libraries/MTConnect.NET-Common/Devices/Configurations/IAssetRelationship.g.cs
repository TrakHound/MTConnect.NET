// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// ConfigurationRelationship that describes the association between a Component and an Asset.
    /// </summary>
    public interface IAssetRelationship : IConfigurationRelationship
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
        /// URI reference to the associated Asset.
        /// </summary>
        string Href { get; }
    }
}