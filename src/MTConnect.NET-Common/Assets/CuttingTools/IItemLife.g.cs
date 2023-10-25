// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Life of a CuttingItem.
    /// </summary>
    public interface IItemLife
    {
        /// <summary>
        /// Indicates if the item life counts from zero to maximum or maximum to zero.
        /// </summary>
        CountDirectionType CountDirection { get; }
        
        /// <summary>
        /// Initial life of the item when it is new.
        /// </summary>
        double Initial { get; }
        
        /// <summary>
        /// End of life limit for this item.
        /// </summary>
        double Limit { get; }
        
        /// <summary>
        /// Type of item life being accumulated.
        /// </summary>
        MTConnect.Assets.CuttingTools.ToolLife Type { get; }
        
        /// <summary>
        /// Value of ItemLife.
        /// </summary>
        double Value { get; }
        
        /// <summary>
        /// Point at which a item life warning will be raised.
        /// </summary>
        double Warning { get; }
    }
}