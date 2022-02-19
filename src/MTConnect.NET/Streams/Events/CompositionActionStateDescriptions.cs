// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
