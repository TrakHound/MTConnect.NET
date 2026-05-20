// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Errors.Xml
{
    /// <summary>
    /// XML serialization surrogate for a single <c>Error</c> element within an
    /// MTConnectError response document.
    /// </summary>
    public class XmlError
    {
        /// <summary>
        /// The standardized MTConnect error code, carried by the
        /// <c>errorCode</c> attribute.
        /// </summary>
        [XmlAttribute("errorCode")]
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// The human-readable error description, carried as the element's text
        /// content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Error"/>,
        /// copying the error code and message.
        /// </summary>
        public IError ToError()
        {
            var error = new Error();
            error.ErrorCode = ErrorCode;
            error.Value = Value;
            return error;
        }
    }
}