// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Part of of the tool that physically removes the material from the workpiece by shear deformation.
    /// </summary>
    public partial interface ICuttingItem
    {
        /// <summary>
        /// Returns a processed copy of this cutting item with its measurements rebound to their concrete tooling-measurement subtypes for serialization.
        /// </summary>
        ICuttingItem Process();
    }
}