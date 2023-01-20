// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ProcessOccurrence is a Component that organizesinformation about the execution of a specific process
    /// that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.
    /// </summary>
    public class ProcessOccurrenceComponent : Component 
    {
        public const string TypeId = "ProcessOccurrence";
        public const string NameId = "prococc";
        public new const string DescriptionText = "ProcessOccurrence is a Component that organizesinformation about the execution of a specific process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ProcessOccurrenceComponent()  { Type = TypeId; }
    }
}