// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.References
{
    /// <summary>
    /// ComponentRef XML element is a pointer to all of the information associated with another Structural Element defined elsewhere in the XML document for a piece of equipment.
    /// ComponentRef allows all of the information (Lower Level Components and all Data Entities) that is associated with the other Structural Element to be directly associated with this XML element.
    /// </summary>
    public class ComponentReference : Reference 
    {
        public new const string DescriptionText = "ComponentRef XML element is a pointer to all of the information associated with another Structural Element defined elsewhere in the XML document for a piece of equipment. ComponentRef allows all of the information (Lower Level Components and all Data Entities) that is associated with the other Structural Element to be directly associated with this XML element.";


        public override string TypeDescription => DescriptionText;
    }
}
