// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices
{
    /// <summary>
    /// Source identifies the physical part of a device where the data represented by the DataItem is originally measured.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// The id attribute of the Component that represents the physical part of a device where teh data represented by the DataItem is actually measured.
        /// </summary>
        [XmlAttribute("componentId")]
        public string ComponentID { get; set; }

        /// <summary>
        /// The id attribute of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        [XmlAttribute("dataItemId")]
        public string DataItemID { get; set; }

        [XmlText]
        public string CDATA { get; set; }
    }
}
