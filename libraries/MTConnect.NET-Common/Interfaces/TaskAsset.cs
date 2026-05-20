// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using System;
using System.Xml.Serialization;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Asset representing a concrete instance of an Interface task, carrying its kind, current lifecycle state, and scheduling precedence.
    /// </summary>
    [XmlRoot("Task")]
    public class TaskAsset : Asset
    {
        /// <summary>
        /// The fixed Asset type identifier ("Task") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public const string TypeId = "Task";


        /// <summary>
        /// The category of work this task performs (e.g. tool change, material load/unload, move material).
        /// </summary>
        public TaskType TaskType { get; set; }

        /// <summary>
        /// The current point of this task in its lifecycle (e.g. committing, active, complete, failed).
        /// </summary>
        public TaskState State { get; set; }

        /// <summary>
        /// The relative precedence used to schedule this task against other pending tasks.
        /// </summary>
		public Priority Priority { get; set; }


        /// <summary>
        /// Initializes a new TaskAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public TaskAsset()
        {
            Type = TypeId;
        }


        /// <summary>
        /// Filters this asset by MTConnect version: returns the asset when the requested version is 1.3 or later (the version that introduced Task), otherwise null to exclude it.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document is being generated for.</param>
        protected override IAsset OnProcess(Version mtconnectVersion)
        {
			if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version13)
			{
				return this;
			}

			return null;
		}
    }
}