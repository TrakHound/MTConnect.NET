// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1658942745281_216676_1135

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configurationrelationship that describes the association between a Component and an Asset.
    /// </summary>
    public class AssetRelationship : Relationship, IAssetRelationship
    {
        public new const string DescriptionText = "Configurationrelationship that describes the association between a Component and an Asset.";


        /// <summary>
        /// Uuid of the related Asset.
        /// </summary>
        public string AssetIdRef { get; set; }
        
        /// <summary>
        /// Type of Asset being referenced.
        /// </summary>
        public string AssetType { get; set; }
        
        /// <summary>
        /// Uri reference to the associated Asset.
        /// </summary>
        public string Href { get; set; }
    }
}