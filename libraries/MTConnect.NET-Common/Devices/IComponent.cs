// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    public partial interface IComponent : IContainer
    {
        /// <summary>
        /// The type of component
        /// </summary>
        string Type { get; }

        bool IsOrganizer { get; }


        /// <summary>
        /// Return a list of All Components
        /// </summary>
        IEnumerable<IComponent> GetComponents();

        /// <summary>
        /// Return the first Component matching the Type
        /// </summary>
        IComponent GetComponent(string type, string name = null, SearchType searchType = SearchType.AnyLevel);

        /// <summary>
        /// Return the first Component matching the Type
        /// </summary>
        IComponent GetComponent<TComponent>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComponent : IComponent;

        /// <summary>
        /// Return All Components matching the Type
        /// </summary>
        IEnumerable<IComponent> GetComponents(string type, string name = null, SearchType searchType = SearchType.AnyLevel);

        /// <summary>
        /// Return All Components matching the Type
        /// </summary>
        IEnumerable<IComponent> GetComponents<TComponent>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComponent : IComponent;


        /// <summary>
        /// Return a list of All Compositions
        /// </summary>
        IEnumerable<IComposition> GetCompositions();

        /// <summary>
        /// Return the first Composition matching the Type
        /// </summary>
        IComposition GetComposition(string type, string name = null, SearchType searchType = SearchType.AnyLevel);

        /// <summary>
        /// Return the first Composition matching the Type
        /// </summary>
        IComposition GetComposition<TComposition>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComposition : IComposition;

        /// <summary>
        /// Return All Compositions matching the Type
        /// </summary>
        IEnumerable<IComposition> GetCompositions(string type, string name = null, SearchType searchType = SearchType.AnyLevel);

        /// <summary>
        /// Return All Compositions matching the Type
        /// </summary>
        IEnumerable<IComposition> GetCompositions<TComposition>(string name = null, SearchType searchType = SearchType.AnyLevel) where TComposition : IComposition;


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