// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public partial interface IComponent : IContainer
    {
        /// <summary>
        /// The type of component
        /// </summary>
        string Type { get; }

        bool IsOrganizer { get; }


        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        IEnumerable<IDataItem> GetDataItems();

        /// <summary>
        /// Return the DataItem matching either the ID, Name, or Source of the specified Key
        /// </summary>
        IDataItem GetDataItemByKey(string dataItemKey);

        /// <summary>
        /// Return a list of All Components
        /// </summary>
        IEnumerable<IComponent> GetComponents();

        /// <summary>
        /// Return a list of All Compositions
        /// </summary>
        IEnumerable<IComposition> GetCompositions();
    }
}