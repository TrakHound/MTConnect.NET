// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Number of times the cutter has been reconditioned.
    /// </summary>
    public interface IReconditionCount
    {
        /// <summary>
        /// Maximum number of times the tool may be reconditioned.
        /// </summary>
        int MaximumCount { get; }
        
        /// <summary>
        /// 
        /// </summary>
        int Value { get; }
    }
}