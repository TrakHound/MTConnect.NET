// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// PhysicalAsset that has a portable platform for handling, storing, or moving materials, parts, blanks etc.
    /// </summary>
    public partial interface IPalletAsset : IPhysicalAsset
    {
        /// <summary>
        /// Actuation type of the Pallet's clamping mechanism.
        /// </summary>
        string ClampingMethod { get; }
        
        /// <summary>
        /// Actuation type of the Pallet's mounting mechanism.
        /// </summary>
        string MountingMethod { get; }
        
        /// <summary>
        /// Identifier of the Pallet.
        /// </summary>
        string PalletId { get; }
        
        /// <summary>
        /// Number or sequence assigned to the Pallet in a group of Pallets.
        /// </summary>
        int PalletNumber { get; }
        
        /// <summary>
        /// Type of Pallet. Common types of pallet include: Process, Warehouse, Shipping, Fixture and Machine.
        /// </summary>
        string Type { get; }
    }
}