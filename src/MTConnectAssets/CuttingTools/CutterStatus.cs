// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public enum CutterStatus
    {
        /// <summary>
        /// A new tool that has not been used or first use. Marks the start of the tool history.
        /// </summary>
        NEW,

        /// <summary>
        /// Indicates the tool is available for use. If this is not present, the tool is currently not ready to be used
        /// </summary>
        AVAILABLE,

        /// <summary>
        /// Indicates the tool is unavailable for use in metal removal. If this is not present, the tool is currently not ready to be used
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// Indicates if this tool is has been committed to a device for use and is not available for use in any other device. If this is not present, this tool has not been allocated for this device and can be used by another device
        /// </summary>
        ALLOCATED,

        /// <summary>
        /// Indicates this Cutting Tool has not been committed to a process and can be allocated.
        /// </summary>
        UNALLOCATED,

        /// <summary>
        /// The tool has been measured.
        /// </summary>
        MEASURED,

        /// <summary>
        /// The cutting tool has been reconditioned. See ReconditionCount for the number of times this cutter has been reconditioned.
        /// </summary>
        RECONDITIONED,

        /// <summary>
        /// The tool is in process and has remaining tool life.
        /// </summary>
        USED,

        /// <summary>
        /// The cutting tool has reached the end of its useful life.
        /// </summary>
        EXPIRED,

        /// <summary>
        /// Premature tool failure.
        /// </summary>
        BROKEN,

        /// <summary>
        /// This cutting tool cannot be used until it is entered into the system.
        /// </summary>
        NOT_REGISTERED,

        /// <summary>
        /// The cutting tool is an indeterminate state. This is the default value.
        /// </summary>
        UNKNOWN
    }
}
