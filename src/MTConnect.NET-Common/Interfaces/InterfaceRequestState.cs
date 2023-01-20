// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public enum InterfaceRequestState
    {
        /// <summary>
        /// The Requester is not ready to make a Request.
        /// </summary>
        NOT_READY,

        /// <summary>
        /// The Requester is prepared to make a Request, but no Request for service is required.
        /// </summary>
        READY,

        /// <summary>
        /// The Requester has initiated a Request for a service and the service has not yet been completed by the Responder.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// CONDITION 1: When the Requester has detected a failure condition, it indicates to the Responder to either not initiate an action or stop its action before it completes by changing its state to FAIL. 
        /// CONDITION 2: If the Responder changes its state to FAIL, the Requester MUST change its state to FAIL. 
        /// ACTIONS: After detecting a failure, the Requester SHOULD NOT change its state to any other value until the Responder has acknowledged the FAIL state by changing its state to FAIL.
        /// Once the FAIL state has been acknowledged by the Responder, the Requester may attempt to clear its FAIL state.
        /// As part of the attempt to clear the FAIL state, the Requester MUSTreset any partial actions that were initiated and attempt to return to acondition where it is again ready to perform a service.
        /// If the recovery is successful, the Requester changes its Request state from FAIL to READY. 
        /// If for some reason the Requester is not again prepared to perform a service, it transitions its state from FAIL to NOT_READY.
        /// </summary>
        FAIL
    }
}