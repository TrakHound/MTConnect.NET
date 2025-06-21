// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727793125239_4425_23553

namespace MTConnect.Assets.Fixture
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FixtureAsset : PhysicalAsset, IFixtureAsset
    {
        public new const string DescriptionText = "";


        /// <summary>
        /// Actuation type of the Fixture's clamping mechanism.
        /// </summary>
        public string ClampingMethod { get; set; }
        
        /// <summary>
        /// Identifier of the Pallet.
        /// </summary>
        public string FixtureId { get; set; }
        
        /// <summary>
        /// Number or sequence assigned to the Fixture in a group of Fixtures.
        /// </summary>
        public int FixtureNumber { get; set; }
        
        /// <summary>
        /// Actuation type of the Fixture's mounting mechanism.
        /// </summary>
        public string MountingMethod { get; set; }
    }
}