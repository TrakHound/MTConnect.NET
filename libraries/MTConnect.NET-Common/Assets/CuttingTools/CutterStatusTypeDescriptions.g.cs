// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class CutterStatusTypeDescriptions
    {
        /// <summary>
        /// Tool is has been committed to a piece of equipment for use and is not available for use in any other piece of equipment.
        /// </summary>
        public const string ALLOCATED = "Tool is has been committed to a piece of equipment for use and is not available for use in any other piece of equipment.";
        
        /// <summary>
        /// Tool is available for use. If this is not present, the tool is currently not ready to be used.
        /// </summary>
        public const string AVAILABLE = "Tool is available for use. If this is not present, the tool is currently not ready to be used.";
        
        /// <summary>
        /// Premature tool failure.
        /// </summary>
        public const string BROKEN = "Premature tool failure.";
        
        /// <summary>
        /// Tool has reached the end of its useful life.
        /// </summary>
        public const string EXPIRED = "Tool has reached the end of its useful life.";
        
        /// <summary>
        /// Tool has been measured.
        /// </summary>
        public const string MEASURED = "Tool has been measured.";
        
        /// <summary>
        /// New tool that has not been used or first use. Marks the start of the tool history.
        /// </summary>
        public const string NEW = "New tool that has not been used or first use. Marks the start of the tool history.";
        
        /// <summary>
        /// Tool cannot be used until it is entered into the system.
        /// </summary>
        public const string NOT_REGISTERED = "Tool cannot be used until it is entered into the system.";
        
        /// <summary>
        /// Tool has been reconditioned.
        /// </summary>
        public const string RECONDITIONED = "Tool has been reconditioned.";
        
        /// <summary>
        /// Tool has not been committed to a process and can be allocated.
        /// </summary>
        public const string UNALLOCATED = "Tool has not been committed to a process and can be allocated.";
        
        /// <summary>
        /// Tool is unavailable for use in metal removal.
        /// </summary>
        public const string UNAVAILABLE = "Tool is unavailable for use in metal removal.";
        
        /// <summary>
        /// Tool is an indeterminate state. This is the default value.
        /// </summary>
        public const string UNKNOWN = "Tool is an indeterminate state. This is the default value.";
        
        /// <summary>
        /// Tool is in process and has remaining tool life.
        /// </summary>
        public const string USED = "Tool is in process and has remaining tool life.";


        public static string Get(CutterStatusType value)
        {
            switch (value)
            {
                case CutterStatusType.ALLOCATED: return "Tool is has been committed to a piece of equipment for use and is not available for use in any other piece of equipment.";
                case CutterStatusType.AVAILABLE: return "Tool is available for use. If this is not present, the tool is currently not ready to be used.";
                case CutterStatusType.BROKEN: return "Premature tool failure.";
                case CutterStatusType.EXPIRED: return "Tool has reached the end of its useful life.";
                case CutterStatusType.MEASURED: return "Tool has been measured.";
                case CutterStatusType.NEW: return "New tool that has not been used or first use. Marks the start of the tool history.";
                case CutterStatusType.NOT_REGISTERED: return "Tool cannot be used until it is entered into the system.";
                case CutterStatusType.RECONDITIONED: return "Tool has been reconditioned.";
                case CutterStatusType.UNALLOCATED: return "Tool has not been committed to a process and can be allocated.";
                case CutterStatusType.UNAVAILABLE: return "Tool is unavailable for use in metal removal.";
                case CutterStatusType.UNKNOWN: return "Tool is an indeterminate state. This is the default value.";
                case CutterStatusType.USED: return "Tool is in process and has remaining tool life.";
            }

            return null;
        }
    }
}