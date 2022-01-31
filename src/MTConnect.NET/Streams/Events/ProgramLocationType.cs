// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// Defines whether the logic or motion program defined by PROGRAM is being executed from the local memory of the controller or from an outside source.
    /// </summary>
    public enum ProgramLocationType
    {
        /// <summary>
        /// Managed by the controller.
        /// </summary>
        LOCAL,

        /// <summary>
        ///  Not managed by the controller.
        /// </summary>
        EXTERNAL
    }
}
