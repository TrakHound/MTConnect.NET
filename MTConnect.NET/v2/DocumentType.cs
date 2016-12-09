// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect
{
    enum DocumentType
    {
        /// <summary>
        /// MTConnectDevices provides the descriptive information about each device served by this Agent and specifies the data items that are available.
        /// </summary>
        MTConnectDevices,

        /// <summary>
        /// MTConnectStreams contains a timeseries of Samples, Events, and Condition from devices and their components.
        /// </summary>
        MTConnectStreams,

        /// <summary>
        /// An MTConnect asset document contains information pertaining to a machine tool asset, something that is not a direct component of the machine and can be relocated to another device during its lifecycle.
        /// </summary>
        MTConnectAssests,

        /// <summary>
        /// An MTConnectError document contains information about an error that occurred in processing the request.
        /// </summary>
        MTConnectError
    }
}
