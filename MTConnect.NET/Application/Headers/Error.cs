// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Xml;

namespace MTConnect.Application.Headers
{
    /// <summary>
    /// Contains the Header information in an MTConnect Error XML document
    /// </summary>
    public class Error
    {
        public Error() { }

        public Error(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            CreationTime = Tools.UTC.FromDateTime(CreationTime);
        }

        // Required
        public long AssetBufferSize { get; set; }
        public long AssetCount { get; set; }
        public long BufferSize { get; set; }
        public DateTime CreationTime { get; set; }
        public long InstanceId { get; set; }
        public string Sender { get; set; }
        public string Version { get; set; }

        // Optional
        public string TestIndicator { get; set; }
        public long NextSequence { get; set; }
        public long LastSequence { get; set; }
        public long FirstSequence { get; set; }
    }
}
