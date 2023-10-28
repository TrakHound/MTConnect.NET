// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Constrained scalar value associated with a cutting tool.
    /// </summary>
    public partial interface IMeasurement
    {
        public string Type { get; set; }
    }
}