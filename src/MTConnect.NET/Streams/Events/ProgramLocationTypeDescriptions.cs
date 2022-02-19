// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// Defines whether the logic or motion program defined by PROGRAM is being executed from the local memory of the controller or from an outside source.
    /// </summary>
    public static class ProgramLocationTypeDescriptions
    {
        /// <summary>
        /// Managed by the controller.
        /// </summary>
        public const string LOCAL = "Managed by the controller.";

        /// <summary>
        /// Not managed by the controller.
        /// </summary>
        public const string EXTERNAL = "Not managed by the controller.";


        public static string Get(ProgramLocationType value)
        {
            switch (value)
            {
                case ProgramLocationType.LOCAL: return LOCAL;
                case ProgramLocationType.EXTERNAL: return EXTERNAL;
            }

            return null;
        }
    }
}
