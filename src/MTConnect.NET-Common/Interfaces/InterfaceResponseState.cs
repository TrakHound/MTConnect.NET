// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public enum InterfaceResponseState
    {
        /// <summary>
        /// The Responder is not ready to perform a service.
        /// </summary>
        NOT_READY,

        /// <summary>
        /// The Responder is prepared to react to a Request, but no Request for service has been detected.
        /// </summary>
        READY,

        /// <summary>
        /// The Responder has detected and accepted a Request for a service and is in the process of performing the service, but the service has not yet been completed.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// CONDITION 1: The Responder has failed while executing the actions required to perform a service and the service has not yet been completed or the Responder has detected that the Requester has unexpectedly changed state.
        /// CONDITION 2: If the Requester changes its state to FAIL, the Responder MUST change its state to FAIL.
        /// ACTIONS: After entering a FAIL state, the Responder SHOULD NOT change its state to any other value until the Requester has acknowledged the FAIL state by changing its state to FAIL.
        /// Once the FAIL state has been acknowledged by the Requester, the Responder may attempt to clear its FAIL state.
        /// As part of the attempt to clear the FAIL state, the Responder MUST reset any partial actions that were initiated and attempt to return to acondition where it is again ready to perform a service. 
        /// If the recovery is successful, the Responder changes its Response state from FAIL to READY. 
        /// If for some reason the Responder is not again prepared to perform a service, it transitions its state from FAIL to NOT_READY.
        /// </summary>
        FAIL,

        /// <summary>
        /// The Responder has completed the actions required to perform the service.
        /// </summary>
        COMPLETE
    }
}