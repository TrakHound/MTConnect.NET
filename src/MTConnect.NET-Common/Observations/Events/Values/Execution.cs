// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The execution status of a component.
    /// </summary>
    public enum Execution
    {
        UNAVAILABLE,

        /// <summary>
        /// The component is ready to execute instructions. It is currently idle.
        /// </summary>
        READY,

        /// <summary>
        /// The component is actively executing an instruction.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The component suspends the execution of the program due to an external signal. Action is required to resume execution.
        /// </summary>
        INTERRUPTED,

        /// <summary>
        /// The component suspends execution while a secondary operation executes. 
        /// Execution resumes automatically once the secondary operation completes.
        /// </summary>
        WAIT,

        /// <summary>
        /// The motion of the active axes are commanded to stop at their current position.
        /// </summary>
        FEED_HOLD,

        /// <summary>
        /// The component program is not READY to execute.
        /// </summary>
        STOPPED,

        /// <summary>
        /// A command from the program has intentionally interrupted execution.
        /// The component MAY have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.
        /// </summary>
        OPTIONAL_STOP,

        /// <summary>
        /// A command from the program has intentionally interrupted execution. 
        /// Action is required to resume execution.
        /// </summary>
        PROGRAM_STOPPED,

        /// <summary>
        /// The program completed execution.
        /// </summary>
        PROGRAM_COMPLETED
    }
}