// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_25A13743_B12C_4c6c_B1DA_8E2EFDD156EF

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.
    /// </summary>
    public abstract partial class Reference : IReference
    {
        public const string DescriptionText = "Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.";


        /// <summary>
        /// Pointer to the id of a DataItem that contains the information to be associated with this entity.
        /// </summary>
        public string DataItemId { get; set; }
        
        /// <summary>
        /// Pointer to the id of an entity that contains the information to be associated with this entity.
        /// </summary>
        public string IdRef { get; set; }
        
        /// <summary>
        /// name of an element or a piece of equipment.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Pointer to the id of a DataItem that contains the information to be associated with this entity.
        /// </summary>
        public string RefDataItemId { get; set; }
    }
}