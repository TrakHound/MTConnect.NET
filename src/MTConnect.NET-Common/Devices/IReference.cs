// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Reference is a pointer to information that is associated with another Structural Element defined elsewhere in the XML document for a piece of equipment. 
    /// That information may be data from the other element or the entire structure of that element.
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// A pointer to the id attribute of the element that contains the information to be associated with this XML element.    
        /// </summary>
        string IdRef { get; }

        /// <summary>
        /// The optional name of the element. Only informative.    
        /// </summary>
        string Name { get; }

        string TypeDescription { get; }
    }
}
