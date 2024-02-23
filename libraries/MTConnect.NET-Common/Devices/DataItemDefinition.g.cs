// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1581433165009_756087_158

namespace MTConnect.Devices
{
    /// <summary>
    /// Representation is either `DATA_SET` or `TABLE`.
    /// </summary>
    public class DataItemDefinition : IDataItemDefinition
    {
        public const string DescriptionText = "Representation is either `DATA_SET` or `TABLE`.";


        /// <summary>
        /// Semantic definition of a Cell.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.ICellDefinition> CellDefinitions { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Semantic definition of an Entry.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.IEntryDefinition> EntryDefinitions { get; set; }
    }
}