// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
