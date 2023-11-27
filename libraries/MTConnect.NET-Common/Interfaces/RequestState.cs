// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public enum RequestState
    {
        /// <summary>
        /// The Requester is not ready to make a Request.
        /// </summary>
        NOT_READY,

        /// <summary>
        /// The Requester is prepared to make a Request, but no Request for service is required. 
        /// The Requester will transition to ACTIVE when it needs a service to be performed.
        /// </summary>
        READY,

        /// <summary>
        /// The Requester has initiated a Request for a service and the service has not yet been completed by the Responder.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The requestor has detected a failure condition and is stopping the action before it completes. 
        /// Fail SHOULD occur after the request is active, but MAY occur after it is READY if the action fails before it can transition to ACTIVE.
        /// </summary>
        FAIL
    }
}