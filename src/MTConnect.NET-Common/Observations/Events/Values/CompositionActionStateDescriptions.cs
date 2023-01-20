// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public static class CompositionActionStateDescriptions
    {
        /// <summary>
        /// The Composition element is not operating.
        /// </summary>
        public const string INACTIVE = "The Composition element is not operating.";

        /// <summary>
        /// The Composition element is operating
        /// </summary>
        public const string ACTIVE = "The Composition element is operating";


        public static string Get(CompositionActionState value)
        {
            switch (value)
            {
                case CompositionActionState.INACTIVE: return INACTIVE;
                case CompositionActionState.ACTIVE: return ACTIVE;
            }

            return null;
        }
    }
}