// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Assets;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A tool storage location associated with a ToolMagazine or AutomaticToolChanger.
    /// </summary>
    public interface IPotModel : ICompositionModel
    {
        CuttingToolModel CuttingTool { get; set; }
    }
}