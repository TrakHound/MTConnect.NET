// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Reference that is a pointer to a DataItem associated with another entity defined for a piece of equipment.
    /// </summary>
    public interface IDataItemRef : IReference
    {
        /// <summary>
        /// Pointer to the id attribute of the DataItem that contains the information to be associated with this element.
        /// </summary>
        MTConnect.Devices.IDataItem IdRef { get; }
    }
}