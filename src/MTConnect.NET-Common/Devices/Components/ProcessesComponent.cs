// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Processes organizes information describing the manufacturing process being executed on a piece of equipment.
    /// </summary>
    public class ProcessesComponent : Component 
    {
        public const string TypeId = "Processes";
        public const string NameId = "proc";
        public new const string DescriptionText = "Processes organizes information describing the manufacturing process being executed on a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ProcessesComponent()  { Type = TypeId; }
    }
}
