// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ExecutionDescriptions
    {
        /// <summary>
        /// Component is ready to execute instructions.It is currently idle.
        /// </summary>
        public const string READY = "Component is ready to execute instructions.It is currently idle.";
        
        /// <summary>
        /// Component is actively executing an instruction.
        /// </summary>
        public const string ACTIVE = "Component is actively executing an instruction.";
        
        /// <summary>
        /// Component suspends the execution of the program due to an external signal.Action is required to resume execution.
        /// </summary>
        public const string INTERRUPTED = "Component suspends the execution of the program due to an external signal.Action is required to resume execution.";
        
        /// <summary>
        /// Motion of the active axes are commanded to stop at their current position.
        /// </summary>
        public const string FEED_HOLD = "Motion of the active axes are commanded to stop at their current position.";
        
        /// <summary>
        /// Component program is not `READY` to execute.
        /// </summary>
        public const string STOPPED = "Component program is not `READY` to execute.";
        
        /// <summary>
        /// Command from the program has intentionally interrupted execution.The Component **MAY** have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.
        /// </summary>
        public const string OPTIONAL_STOP = "Command from the program has intentionally interrupted execution.The Component **MAY** have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.";
        
        /// <summary>
        /// Command from the program has intentionally interrupted execution.Action is required to resume execution.
        /// </summary>
        public const string PROGRAM_STOPPED = "Command from the program has intentionally interrupted execution.Action is required to resume execution.";
        
        /// <summary>
        /// Program completed execution.
        /// </summary>
        public const string PROGRAM_COMPLETED = "Program completed execution.";
        
        /// <summary>
        /// Component suspends execution while a secondary operation executes.Execution resumes automatically once the secondary operation completes.
        /// </summary>
        public const string WAIT = "Component suspends execution while a secondary operation executes.Execution resumes automatically once the secondary operation completes.";
        
        /// <summary>
        /// Program has been intentionally optionally stopped using an M01 or similar code.**DEPRECATED** in *version 1.4* and replaced with `OPTIONAL_STOP`.
        /// </summary>
        public const string PROGRAM_OPTIONAL_STOP = "Program has been intentionally optionally stopped using an M01 or similar code.**DEPRECATED** in *version 1.4* and replaced with `OPTIONAL_STOP`.";


        public static string Get(Execution value)
        {
            switch (value)
            {
                case Execution.READY: return "Component is ready to execute instructions.It is currently idle.";
                case Execution.ACTIVE: return "Component is actively executing an instruction.";
                case Execution.INTERRUPTED: return "Component suspends the execution of the program due to an external signal.Action is required to resume execution.";
                case Execution.FEED_HOLD: return "Motion of the active axes are commanded to stop at their current position.";
                case Execution.STOPPED: return "Component program is not `READY` to execute.";
                case Execution.OPTIONAL_STOP: return "Command from the program has intentionally interrupted execution.The Component **MAY** have another state that indicates if the execution is interrupted or the execution ignores the interrupt instruction.";
                case Execution.PROGRAM_STOPPED: return "Command from the program has intentionally interrupted execution.Action is required to resume execution.";
                case Execution.PROGRAM_COMPLETED: return "Program completed execution.";
                case Execution.WAIT: return "Component suspends execution while a secondary operation executes.Execution resumes automatically once the secondary operation completes.";
                case Execution.PROGRAM_OPTIONAL_STOP: return "Program has been intentionally optionally stopped using an M01 or similar code.**DEPRECATED** in *version 1.4* and replaced with `OPTIONAL_STOP`.";
            }

            return null;
        }
    }
}