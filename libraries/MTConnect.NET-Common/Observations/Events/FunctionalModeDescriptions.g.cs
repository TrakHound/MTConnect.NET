// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class FunctionalModeDescriptions
    {
        /// <summary>
        /// Component is currently producing product, ready to produce product, or its current intended use is to be producing product.
        /// </summary>
        public const string PRODUCTION = "Component is currently producing product, ready to produce product, or its current intended use is to be producing product.";
        
        /// <summary>
        /// Component is not currently producing product. It is being prepared or modified to begin production of product.
        /// </summary>
        public const string SETUP = "Component is not currently producing product. It is being prepared or modified to begin production of product.";
        
        /// <summary>
        /// Component is not currently producing product.Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.
        /// </summary>
        public const string TEARDOWN = "Component is not currently producing product.Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.";
        
        /// <summary>
        /// Component is not currently producing product.It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.
        /// </summary>
        public const string MAINTENANCE = "Component is not currently producing product.It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.";
        
        /// <summary>
        /// Component is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.
        /// </summary>
        public const string PROCESS_DEVELOPMENT = "Component is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.";


        public static string Get(FunctionalMode value)
        {
            switch (value)
            {
                case FunctionalMode.PRODUCTION: return "Component is currently producing product, ready to produce product, or its current intended use is to be producing product.";
                case FunctionalMode.SETUP: return "Component is not currently producing product. It is being prepared or modified to begin production of product.";
                case FunctionalMode.TEARDOWN: return "Component is not currently producing product.Typically, it has completed the production of a product and is being modified or returned to a neutral state such that it may then be prepared to begin production of a different product.";
                case FunctionalMode.MAINTENANCE: return "Component is not currently producing product.It is currently being repaired, waiting to be repaired, or has not yet been returned to a normal production status after maintenance has been performed.";
                case FunctionalMode.PROCESS_DEVELOPMENT: return "Component is being used to prove-out a new process, testing of equipment or processes, or any other active use that does not result in the production of product.";
            }

            return null;
        }
    }
}