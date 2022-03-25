// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current intended production status of the device or component.
    /// </summary>
    public static class FunctionalModeDescriptions
    {
        /// <summary>
        /// The Device element or another Structural Element is currently producing product, ready to produce product, or its current intended use is to be producing product.
        /// </summary>
        public const string PRODUCTION = "The Device element or another Structural Element is currently producing product, ready to produce product, or its current intended use is to be producing product.";

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. It is being prepared or modified to begin production of product.
        /// </summary>
        public const string SETUP = "The Device element or another Structural Element is not currently producing product. It is being prepared or modified to begin production of product.";

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. 
        /// Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.
        /// </summary>
        public const string TEAR_DOWN = "The Device element or another Structural Element is not currently producing product. Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.";

        /// <summary>
        /// The Device element or another Structural Element is not currently producing product. 
        /// It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.
        /// </summary>
        public const string MAINTENANCE = "The Device element or another Structural Element is not currently producing product. It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.";

        /// <summary>
        /// The Device element or another Structural Element is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.
        /// </summary>
        public const string PROCESS_DEVELOPMENT = "The Device element or another Structural Element is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.";


        public static string Get(FunctionalMode value)
        {
            switch (value)
            {
                case FunctionalMode.PRODUCTION: return PRODUCTION;
                case FunctionalMode.SETUP: return SETUP;
                case FunctionalMode.TEAR_DOWN: return TEAR_DOWN;
                case FunctionalMode.MAINTENANCE: return MAINTENANCE;
                case FunctionalMode.PROCESS_DEVELOPMENT: return PROCESS_DEVELOPMENT;
            }

            return null;
        }
    }
}
