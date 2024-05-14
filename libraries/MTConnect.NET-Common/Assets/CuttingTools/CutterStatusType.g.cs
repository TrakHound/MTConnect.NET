// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public enum CutterStatusType
    {
        /// <summary>
        /// Tool is has been committed to a piece of equipment for use and is not available for use in any other piece of equipment.
        /// </summary>
        ALLOCATED,
        
        /// <summary>
        /// Tool is available for use. If this is not present, the tool is currently not ready to be used.
        /// </summary>
        AVAILABLE,
        
        /// <summary>
        /// Premature tool failure.
        /// </summary>
        BROKEN,
        
        /// <summary>
        /// Tool has reached the end of its useful life.
        /// </summary>
        EXPIRED,
        
        /// <summary>
        /// Tool has been measured.
        /// </summary>
        MEASURED,
        
        /// <summary>
        /// New tool that has not been used or first use. Marks the start of the tool history.
        /// </summary>
        NEW,
        
        /// <summary>
        /// Tool cannot be used until it is entered into the system.
        /// </summary>
        NOT_REGISTERED,
        
        /// <summary>
        /// Tool has been reconditioned.
        /// </summary>
        RECONDITIONED,
        
        /// <summary>
        /// Tool has not been committed to a process and can be allocated.
        /// </summary>
        UNALLOCATED,
        
        /// <summary>
        /// Tool is unavailable for use in metal removal.
        /// </summary>
        UNAVAILABLE,
        
        /// <summary>
        /// Tool is an indeterminate state. This is the default value.
        /// </summary>
        UNKNOWN,
        
        /// <summary>
        /// Tool is in process and has remaining tool life.
        /// </summary>
        USED
    }
}