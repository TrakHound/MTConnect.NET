// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public enum ResponseState
    {
        /// <summary>
        /// The Responder is not ready to perform a service.
        /// </summary>
        NOT_READY,

        /// <summary>
        /// The Responder is prepared to react to a Request, but no Request for service has been detected.
        /// The Responder MUST transition to ACTIVE to inform the Requester that it has detected and accepted the Request and is in the process of performing the requested service.
        /// If the Responder is not ready to perform a Request, it MUST transition to a NOT_READY state.
        /// </summary>
        READY,

        /// <summary>
        /// The Responder has detected and accepted a Request for a service and is in the process of performing the service, but the service has not yet been completed.
        /// In normal operation, the Responder MUST NOT change its state to ACTIVE unless the Requester state is ACTIVE.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The response has failed to perform the action. The failure SHOULD wait for the request to acknowledge the FAIL state and transition back to READY or NOT_READY.
        /// </summary>
        FAIL,

        /// <summary>
        /// The Responder has completed the actions required to perform the service.
        /// </summary>
        COMPLETE
    }
}