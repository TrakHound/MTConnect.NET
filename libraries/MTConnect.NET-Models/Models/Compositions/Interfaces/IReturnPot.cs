// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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