// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;

namespace MTConnect.Assets
{
    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        DateTime Timestamp { get; set; }

        /// <summary>
        /// The piece of equipments UUID that supplied this data.
        /// </summary>
        string DeviceUuid { get; set; }

        /// <summary>
        /// This is an optional attribute that is an indicator that the MTConnect
        /// Asset has been removed from the piece of equipment.
        /// </summary>
        bool Removed { get; set; }

        /// <summary>
        /// An optional element that can contain any descriptive content.
        /// </summary>
        string Description { get; set; }

        string Xml { get; set; }


        IAsset Process(Version mtconnectVersion);

        AssetValidationResult IsValid(Version mtconnectVersion);
    }
}