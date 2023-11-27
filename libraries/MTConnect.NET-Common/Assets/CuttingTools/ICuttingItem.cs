// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Part of of the tool that physically removes the material from the workpiece by shear deformation.
    /// </summary>
    public partial interface ICuttingItem
    {
        ICuttingItem Process();
    }
}