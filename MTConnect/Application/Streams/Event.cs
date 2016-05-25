// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml;

namespace MTConnect.Streams
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Event XML elements representing EVENT category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Event XML Elements in an Events container.
    /// </summary>
    public class Event : DataItem
    {
        public Event() { }

        public Event(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            FullAddress = Tools.Address.GetStreams(node);
            Category = DataItemCategory.EVENT;
            Type = node.Name;
            CDATA = node.InnerText;
        }
    }
}
