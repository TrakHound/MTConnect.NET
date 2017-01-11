// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.MTConnectError
{
    public enum ErrorCode
    {
        /// <summary>
        /// The request did not have sufficient permissions to perform the request.
        /// </summary>
        UNAUTHORIZED,

        /// <summary>
        /// The device specified in the URI could not be found.
        /// </summary>
        NO_DEVICE,

        /// <summary>
        /// The sequence number was beyond the end of the buffer.
        /// </summary>
        OUT_OF_RANGE,

        /// <summary>
        /// The count given is too large.
        /// </summary>
        TOO_MANY,

        /// <summary>
        /// The URI provided was incorrect.
        /// </summary>
        INVALID_URI,

        /// <summary>
        /// The request was not one of the three specified requests.
        /// </summary>
        INVALID_REQUEST,

        /// <summary>
        /// Contact the software provider, the Agent did not behave correctly.
        /// </summary>
        INTERNAL_ERROR,

        /// <summary>
        /// The XPath could not be parsed. Invalid syntax or XPath did not match any valid elements in the document.
        /// </summary>
        INVALID_XPATH,

        /// <summary>
        /// A valid request was provided, but the Agent does not support the feature or request type.
        /// </summary>
        UNSUPPORTED,

        /// <summary>
        /// An asset ID cannot be located.
        /// </summary>
        ASSET_NOT_FOUND
    }
}
