// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1582939685398_830533_4339

namespace MTConnect.Devices
{
    /// <summary>
    /// Semantic definition of an Entry.
    /// </summary>
    public class EntryDefinition : IEntryDefinition
    {
        public const string DescriptionText = "Semantic definition of an Entry.";


        /// <summary>
        /// Semantic definition of a Cell.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.ICellDefinition> CellDefinitions { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Unique identification of the Entry in the Definition. key.
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