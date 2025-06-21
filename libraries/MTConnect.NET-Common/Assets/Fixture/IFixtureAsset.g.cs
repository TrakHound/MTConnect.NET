// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Fixture
{
    /// <summary>
    /// 
    /// </summary>
    public partial interface IFixtureAsset : IPhysicalAsset
    {
        /// <summary>
        /// Actuation type of the Fixture's clamping mechanism.
        /// </summary>
        string ClampingMethod { get; }
        
        /// <summary>
        /// Identifier of the Pallet.
        /// </summary>
        string FixtureId { get; }
        
        /// <summary>
        /// Number or sequence assigned to the Fixture in a group of Fixtures.
        /// </summary>
        int FixtureNumber { get; }
        
        /// <summary>
        /// Actuation type of the Fixture's mounting mechanism.
        /// </summary>
        string MountingMethod { get; }
    }
}