// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Xml;

namespace MTConnect.Headers
{
    /// <summary>
    /// Contains the Header information in an MTConnect Assets XML document
    /// </summary>
    public class Assets
    {
        public Assets() { }

        public Assets(XmlNode HeaderNode)
        {
            Tools.XML.AssignProperties(this, HeaderNode);
            CreationTime = Tools.UTC.FromDateTime(CreationTime);
        }

        // Required
        public long AssetBufferSize { get; set; }
        public long AssetCount { get; set; }
        public DateTime CreationTime { get; set; }
        public long InstanceId { get; set; }
        public string Sender { get; set; }
        public string Version { get; set; }

        // Optional
        public string TestIndicator { get; set; }
    }
}
