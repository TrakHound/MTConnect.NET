// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Operating state of a Component.
    /// </summary>
    public enum Execution
    {
        /// <summary>
        /// Component is ready to execute instructions.It is currently idle.
        /// </summary>
        READY,
        
        /// <summary>
        /// Component is actively executing an instruction.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Component suspends the execution of the program due to an external signal.Action is required to resume execution.
        /// </summary>
        INTERRUPTED,
        
        /// <summary>
        /// Motion of the active axes are commanded to stop at their current position.
        /// </summary>
        FEED_HOLD,
        
        /// <summary>
        /// Component program is not `READY` to execute.
        /// </summary>
        STOPPED,
        
        /// <summary>
        /// Command from the program has intentionally interrupted execution.The Component **MAY** have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.
        /// </summary>
        OPTIONAL_STOP,
        
        /// <summary>
        /// Command from the program has intentionally interrupted execution.Action is required to resume execution.
        /// </summary>
        PROGRAM_STOPPED,
        
        /// <summary>
        /// Program completed execution.
        /// </summary>
        PROGRAM_COMPLETED,
        
        /// <summary>
        /// Component suspends execution while a secondary operation executes.Execution resumes automatically once the secondary operation completes.
        /// </summary>
        WAIT,
        
        /// <summary>
        /// Program has been intentionally optionally stopped using an M01 or similar code.**DEPRECATED** in *version 1.4* and replaced with `OPTIONAL_STOP`.
        /// </summary>
        PROGRAM_OPTIONAL_STOP
    }
}