// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="CompositionStateAction"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class CompositionStateActionDescriptions
    {
        /// <summary>
        /// Composition is operating.
        /// </summary>
        public const string ACTIVE = "Composition is operating.";
        
        /// <summary>
        /// Composition is not operating.
        /// </summary>
        public const string INACTIVE = "Composition is not operating.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="CompositionStateAction"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(CompositionStateAction value)
        {
            switch (value)
            {
                case CompositionStateAction.ACTIVE: return "Composition is operating.";
                case CompositionStateAction.INACTIVE: return "Composition is not operating.";
            }

            return null;
        }
    }
}