// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml;

namespace MTConnect.MTConnectStreams
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Event XML elements representing EVENT category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Event XML Elements in an Events container.
    /// </summary>
    public class Event : DataItem
    {
        public Event()
        {
            Category = DataItemCategory.EVENT;
        }
    }
}
