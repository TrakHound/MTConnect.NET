// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;

namespace MTConnect.MTConnectError
{
    public class Error
    {
        [XmlText]
        public string CDATA { get; set; }

        [XmlAttribute("errorCode")]
        public ErrorCode ErrorCode { get; set; }
    }
}
