// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using System;
using System.Xml.Serialization;

namespace MTConnect.Interfaces
{
    [XmlRoot("Task")]
    public class TaskAsset : Asset
    {
        public const string TypeId = "Task";


        public TaskType TaskType { get; set; }

        public TaskState State { get; set; }

		public Priority Priority { get; set; }


        public TaskAsset()
        {
            Type = TypeId;
        }


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