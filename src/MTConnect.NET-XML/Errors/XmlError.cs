// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;

namespace MTConnect.Errors.Xml
{
    public class XmlError
    {
        [XmlAttribute("errorCode")]
        public ErrorCode ErrorCode { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IError ToError()
        {
            var error = new Error();
            error.ErrorCode = ErrorCode;
            error.Value = Value;
            return error;
        }
    }
}
