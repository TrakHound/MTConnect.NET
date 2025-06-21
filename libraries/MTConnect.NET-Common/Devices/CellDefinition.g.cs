// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1581433195808_917937_222

namespace MTConnect.Devices
{
    /// <summary>
    /// Semantic definition of a Cell.
    /// </summary>
    public class CellDefinition : ICellDefinition
    {
        public const string DescriptionText = "Semantic definition of a Cell.";


        /// <summary>
        /// Textual description for CellDefinition.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Unique identification of the Cell in the Definition. key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Key.
        /// </summary>
        public string KeyType { get; set; }
        
        /// <summary>
        /// SubType. See DataItem.
        /// </summary>
        public string SubType { get; set; }
        
        /// <summary>
        /// Type. See DataItem Types.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Units. See Value Properties of DataItem.
        /// </summary>
        public string Units { get; set; }
    }
}