// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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