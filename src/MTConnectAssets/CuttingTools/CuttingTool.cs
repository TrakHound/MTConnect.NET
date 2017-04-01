// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;


namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class CuttingTool : Asset
    {
        #region "Required"

        /// <summary>
        /// The unique identifier for this assembly. 
        /// This is defined as an XML string type and is implementation dependent.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The identifier for a class of cutting tools. 
        /// This is defined as an XML string type and is implementation dependent.
        /// </summary>
        [XmlAttribute("toolId")]
        public string ToolId { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// The manufacturers of the cutting tool. 
        /// An optional attribute referring to the manufacturers of this tool, for this element, this will reference the Tool Item and Adaptive Items specifically. 
        /// The Cutting Items manufacturers’ will be an attribute of the CuttingItem elements. 
        /// The representation will be a comma (,) delimited list of manufacturer names. This can be any series of numbers and letters as defined by the XML type string.
        /// </summary>
        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The device’s UUID that supplied this data. 
        /// This optional element References to the UUID attribute given in the device element. 
        /// This can be any series of numbers and letters as defined by the XML type NMTOKEN.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// This is an indicator that the cutting tool has been removed from the device. 
        /// If the asset is marked as removed, it will not be visible to the client application unless the includeRemoved=true parameter is provided in the URL. 
        /// If this attribute is not present it MUST be assumed to be false. 
        /// The value is an xsi:boolean type and MUST be true or false.
        /// </summary>
        [XmlAttribute("removed")]
        public bool Removed { get; set; }

        #endregion

        [XmlElement("Description")]
        public Description Description { get; set; }

        [XmlElement("CuttingToolLifeCycle")]
        public CuttingToolLifeCycle CuttingToolLifeCycle { get; set; }
    }
}
