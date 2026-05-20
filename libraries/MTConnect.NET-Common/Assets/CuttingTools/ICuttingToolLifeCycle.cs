// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
    /// </summary>
    public partial interface ICuttingToolLifeCycle
    {
        /// <summary>
        /// Returns a processed copy of this life cycle with its cutting items processed and measurements rebound to their concrete tooling-measurement subtypes for serialization.
        /// </summary>
        ICuttingToolLifeCycle Process();
    }
}