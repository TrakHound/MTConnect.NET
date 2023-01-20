// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public interface IComponent : IContainer
    {
        bool IsOrganizer { get; }


        /// <summary>
        /// A container for the Component elements associated with this Component element.
        /// </summary>
        IEnumerable<IComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        IEnumerable<IComposition> Compositions { get; set; }


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
