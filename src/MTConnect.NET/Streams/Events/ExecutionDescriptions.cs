// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// The execution status of a component.
    /// </summary>
    public static class ExecutionDescriptions
    {
        /// <summary>
        /// The component is ready to execute instructions. It is currently idle.
        /// </summary>
        public const string READY = "The component is ready to execute instructions. It is currently idle.";

        /// <summary>
        /// The component is actively executing an instruction.
        /// </summary>
        public const string ACTIVE = "The component is actively executing an instruction.";

        /// <summary>
        /// The component suspends the execution of the program due to an external signal. Action is required to resume execution.
        /// </summary>
        public const string INTERRUPTED = "The component suspends the execution of the program due to an external signal. Action is required to resume execution.";

        /// <summary>
        /// The component suspends execution while a secondary operation executes. 
        /// Execution resumes automatically once the secondary operation completes.
        /// </summary>
        public const string WAIT = "The component suspends execution while a secondary operation executes. Execution resumes automatically once the secondary operation completes.";

        /// <summary>
        /// The motion of the active axes are commanded to stop at their current position.
        /// </summary>
        public const string FEED_HOLD = "The motion of the active axes are commanded to stop at their current position.";

        /// <summary>
        /// The component program is not READY to execute.
        /// </summary>
        public const string STOPPED = "The component program is not READY to execute.";

        /// <summary>
        /// A command from the program has intentionally interrupted execution.
        /// The component MAY have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.
        /// </summary>
        public const string OPTIONAL_STOP = "A command from the program has intentionally interrupted execution. The component MAY have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.";

        /// <summary>
        /// A command from the program has intentionally interrupted execution. 
        /// Action is required to resume execution.
        /// </summary>
        public const string PROGRAM_STOPPED = "A command from the program has intentionally interrupted execution. Action is required to resume execution.";

        /// <summary>
        /// The program completed execution.
        /// </summary>
        public const string PROGRAM_COMPLETED = "The program completed execution.";


        public static string Get(Execution value)
        {
            switch (value)
            {
                case Execution.READY: return READY;
                case Execution.ACTIVE: return ACTIVE;
                case Execution.INTERRUPTED: return INTERRUPTED;
                case Execution.WAIT: return WAIT;
                case Execution.FEED_HOLD: return FEED_HOLD;
                case Execution.STOPPED: return STOPPED;
                case Execution.OPTIONAL_STOP: return OPTIONAL_STOP;
                case Execution.PROGRAM_STOPPED: return PROGRAM_STOPPED;
                case Execution.PROGRAM_COMPLETED: return PROGRAM_COMPLETED;
            }

            return null;
        }
    }
}
