// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727793137466_630771_23584

namespace MTConnect.Assets.Pallet
{
    /// <summary>
    /// PhysicalAsset that has a portable platform for handling, storing, or moving materials, parts, blanks etc.
    /// </summary>
    public partial class PalletAsset : PhysicalAsset, IPalletAsset
    {
        public new const string DescriptionText = "PhysicalAsset that has a portable platform for handling, storing, or moving materials, parts, blanks etc.";


        /// <summary>
        /// Actuation type of the Pallet's clamping mechanism.
        /// </summary>
        public string ClampingMethod { get; set; }
        
        /// <summary>
        /// Actuation type of the Pallet's mounting mechanism.
        /// </summary>
        public string MountingMethod { get; set; }
        
        /// <summary>
        /// Identifier of the Pallet.
        /// </summary>
        public string PalletId { get; set; }
        
        /// <summary>
        /// Number or sequence assigned to the Pallet in a group of Pallets.
        /// </summary>
        public int PalletNumber { get; set; }
        
        /// <summary>
        /// Type of Pallet. Common types of pallet include: Process, Warehouse, Shipping, Fixture and Machine.
        /// </summary>
        public string Type { get; set; }
    }
}