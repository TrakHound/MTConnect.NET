// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Interfaces
{
    [XmlRoot("TaskArchetype")]
    public abstract class TaskArchetypeAsset : Asset
    {
        public const string TypeId = "TaskArchetype";


        public TaskType TaskType { get; set; }

        public Priority Priority { get; set; }

        public IEnumerable<TaskArchetypeAsset> SubTaskRef { get; set; }


        public TaskArchetypeAsset()
        {
            Type = TypeId;
        }
    }
}