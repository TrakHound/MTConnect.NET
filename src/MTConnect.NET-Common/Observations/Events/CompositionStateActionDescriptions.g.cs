// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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