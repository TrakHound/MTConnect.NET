// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Asset describing a reusable template for an Interface task: the kind of work, its precedence, and the subtasks it decomposes into.
    /// Concrete task instances are derived from an archetype.
    /// </summary>
    [XmlRoot("TaskArchetype")]
    public abstract class TaskArchetypeAsset : Asset
    {
        /// <summary>
        /// The fixed Asset type identifier ("TaskArchetype") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public const string TypeId = "TaskArchetype";


        /// <summary>
        /// The category of work this archetype represents (e.g. tool change, material load/unload, move material).
        /// </summary>
        public TaskType TaskType { get; set; }

        /// <summary>
        /// The relative precedence applied when scheduling tasks created from this archetype against other pending tasks.
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// The ordered set of child task archetypes this task decomposes into.
        /// </summary>
        public IEnumerable<TaskArchetypeAsset> SubTaskRef { get; set; }


        /// <summary>
        /// Initializes a new TaskArchetypeAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public TaskArchetypeAsset()
        {
            Type = TypeId;
        }
    }
}