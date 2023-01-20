// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Reference is a pointer to information that is associated with another Structural Element defined elsewhere in the XML document for a piece of equipment. 
    /// That information may be data from the other element or the entire structure of that element.
    /// </summary>
    public class Reference : IReference
    {
        public const string DescriptionText = "Reference is a pointer to information that is associated with another Structural Element defined elsewhere in the XML document for a piece of equipment. That information may be data from the other element or the entire structure of that element.";


        /// <summary>
        /// A pointer to the id attribute of the element that contains the information to be associated with this XML element.    
        /// </summary>
        public string IdRef { get; set; }

        /// <summary>
        /// The optional name of the element. Only informative.    
        /// </summary>
        public string Name { get; set; }

        public virtual string TypeDescription => DescriptionText;
    }
}