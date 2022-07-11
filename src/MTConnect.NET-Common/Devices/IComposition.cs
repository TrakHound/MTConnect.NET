// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// Composition XML elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.
    /// </summary>
    public interface IComposition : IContainer
    {
        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        IEnumerable<IDataItem> GetDataItems();

        /// <summary>
        /// Return the DataItem matching either the ID, Name, or Source of the specified Key
        /// </summary>
        IDataItem GetDataItemByKey(string dataItemKey);
    }
}
