// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class Event : DataItem
    {
        [XmlIgnore]
        [JsonIgnore]
        public bool IsDataSet { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsTable { get; set; }


        public Event()
        {
            Category = Devices.DataItemCategory.EVENT;
        }
    }
}
