// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
