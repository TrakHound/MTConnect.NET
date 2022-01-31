// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Errors
{
    public class Error
    {
        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA { get; set; }

        [XmlAttribute("errorCode")]
        [JsonPropertyName("errorCode")]
        public ErrorCode ErrorCode { get; set; }


        public Error() { }

        public Error(ErrorCode errorCode, string cdata = null)
        {
            ErrorCode = errorCode;
            CDATA = cdata;
        }
    }
}
