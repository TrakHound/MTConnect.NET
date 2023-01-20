// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Assets;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A POT for a tool awaiting transfer from a ToolMagazine to Spindle or Turret.
    /// </summary>
    public interface ITransferPotModel : ICompositionModel
    {
        CuttingToolModel CuttingTool { get; set; }
    }
}