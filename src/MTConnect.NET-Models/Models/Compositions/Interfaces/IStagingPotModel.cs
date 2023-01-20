// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Assets;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A POT for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
    /// </summary>
    public interface IStagingPotModel : ICompositionModel
    {
        CuttingToolModel CuttingTool { get; set; }
    }
}
