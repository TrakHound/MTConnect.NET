// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    public partial interface IComposition : IContainer
    {
        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        IEnumerable<IDataItem> GetDataItems();

        /// <summary>
        /// Return the DataItem matching either the ID, Name, or Source of the specified Key
        /// </summary>
        IDataItem GetDataItemByKey(string dataItemKey);

        /// <summary>
        /// Return the first DataItem matching the Type
        /// </summary>
        IDataItem GetDataItemByType(string type, SearchType searchType = SearchType.Child);

        /// <summary>
        /// Return the first DataItem matching the Type and SubType
        /// </summary>
        IDataItem GetDataItemByType(string type, string subType, SearchType searchType = SearchType.Child);

        /// <summary>
        /// Return the first DataItem matching the Type and SubType
        /// </summary>
        IDataItem GetDataItem<TDataItem>(string subType = null, SearchType searchType = SearchType.Child) where TDataItem : IDataItem;

        /// <summary>
        /// Return All DataItems matching the Type
        /// </summary>
        IEnumerable<IDataItem> GetDataItemsByType(string type, SearchType searchType = SearchType.Child);

        /// <summary>
        /// Return All DataItems matching the Type and SubType
        /// </summary>
        IEnumerable<IDataItem> GetDataItemsByType(string type, string subType, SearchType searchType = SearchType.Child);

        /// <summary>
		/// Return All DataItems matching the Type and SubType
        /// </summary>
        IEnumerable<IDataItem> GetDataItems<TDataItem>(string subType = null, SearchType searchType = SearchType.Child) where TDataItem : IDataItem;
    }
}