// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// A reference from a task archetype to one of its constituent subtasks, describing how that subtask is sequenced and grouped relative to its siblings.
    /// </summary>
    public class SubTaskRef
    {
        /// <summary>
        /// When true, this subtask may run concurrently with other subtasks in the same group rather than strictly in <see cref="Order"/>.
        /// </summary>
        public bool Parallel { get; set; }

        /// <summary>
        /// The name of the group this subtask belongs to, used to cluster subtasks that are sequenced or parallelized together.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The position of this subtask within its group, determining sequential execution order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// When true, the parent task can complete even if this subtask is skipped or not performed.
        /// </summary>
        public bool Optional { get; set; }
    }
}
