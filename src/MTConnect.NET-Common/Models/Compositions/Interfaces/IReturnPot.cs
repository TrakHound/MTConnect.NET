// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Assets;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A POT for a tool removed from Spindle or Turret and awaiting for return to a ToolMagazine.
    /// </summary>
    public interface IReturnPotModel : ICompositionModel
    {
        CuttingToolModel CuttingTool { get; set; }
    }
}
