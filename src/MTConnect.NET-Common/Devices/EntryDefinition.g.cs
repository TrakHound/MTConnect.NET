// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1582939685398_830533_4339

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
        public MTConnect.Devices.ICellDefinition CellDefinition { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Unique identification of the Entry in the Definition. The description applies to all Entry observation having this key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Dataitem type that defines the meaning of the key.
        /// </summary>
        public DataItemType KeyType { get; set; }
        
        /// <summary>
        /// Same as DataItem DataItem::subType. See DataItem.
        /// </summary>
        public DataItemSubType SubType { get; set; }
        
        /// <summary>
        /// Same as DataItem DataItem::type. See DataItem Types.
        /// </summary>
        public DataItemType Type { get; set; }
        
        /// <summary>
        /// Same as DataItem DataItem::units. See Value Properties of DataItem.
        /// </summary>
        public string Units { get; set; }
    }
}