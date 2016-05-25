// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Xml;

namespace MTConnect.Headers
{
    /// <summary>
    /// Contains the Header information in an MTConnect Streams XML document
    /// </summary>
    public class Streams
    {
        public Streams() { }

        public Streams(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            CreationTime = Tools.UTC.FromDateTime(CreationTime);
        }

        // Required
        public long BufferSize { get; set; }
        public DateTime CreationTime { get; set; }
        public long InstanceId { get; set; }
        public string Sender { get; set; }
        public string Version { get; set; }

        public long FirstSequence { get; set; }
        public long LastSequence { get; set; }
        public long NextSequence { get; set; }

        // Optional
        public string TestIndicator { get; set; }
    }
}
