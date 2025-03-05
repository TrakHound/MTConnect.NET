// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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