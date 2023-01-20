// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current intended production status of the device or component.
    /// </summary>
    public enum FunctionalMode
    {
        UNAVAILABLE,

        /// <summary>
        /// The Device element or another Structural Element is currently producing product, ready to produce product, or its current intended use is to be producing product.
        /// </summary>
        PRODUCTION,

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. It is being prepared or modified to begin production of product.
        /// </summary>
        SETUP,

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. 
        /// Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.
        /// </summary>
        TEAR_DOWN,

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. 
        /// It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.
        /// </summary>
        MAINTENANCE,

        /// <summary>
        /// The Device element or another Structural Element is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.
        /// </summary>
        PROCESS_DEVELOPMENT
    }
}