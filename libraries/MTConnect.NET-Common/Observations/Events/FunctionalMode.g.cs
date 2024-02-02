// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Current intended production status of the Component.
    /// </summary>
    public enum FunctionalMode
    {
        /// <summary>
        /// Component is currently producing product, ready to produce product, or its current intended use is to be producing product.
        /// </summary>
        PRODUCTION,
        
        /// <summary>
        /// Component is not currently producing product. It is being prepared or modified to begin production of product.
        /// </summary>
        SETUP,
        
        /// <summary>
        /// Component is not currently producing product.Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.
        /// </summary>
        TEARDOWN,
        
        /// <summary>
        /// Component is not currently producing product.It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.
        /// </summary>
        MAINTENANCE,
        
        /// <summary>
        /// Component is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.
        /// </summary>
        PROCESS_DEVELOPMENT
    }
}