// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_CC4F8633_BAAC_47e8_9EFB_2BFC62215FC8

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Life of a CuttingItem.
    /// </summary>
    public class ItemLife : IItemLife
    {
        public const string DescriptionText = "Life of a CuttingItem.";


        /// <summary>
        /// Indicates if the item life counts from zero to maximum or maximum to zero.
        /// </summary>
        public MTConnect.Assets.CuttingTools.CountDirectionType CountDirection { get; set; }
        
        /// <summary>
        /// Initial life of the item when it is new.
        /// </summary>
        public double? Initial { get; set; }
        
        /// <summary>
        /// End of life limit for this item.
        /// </summary>
        public double? Limit { get; set; }
        
        /// <summary>
        /// Type of item life being accumulated.
        /// </summary>
        public MTConnect.Assets.CuttingTools.ToolLifeType Type { get; set; }
        
        /// <summary>
        /// Value of ItemLife.
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// Point at which a item life warning will be raised.
        /// </summary>
        public double? Warning { get; set; }
    }
}