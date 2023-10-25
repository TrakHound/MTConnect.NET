// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1607346168906_610073_2052

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Process types.
    /// </summary>
    public class ProcessesComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Processes";
        public const string NameId = "processesComponent";
        public new const string DescriptionText = "Component that organize Process types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ProcessesComponent() { Type = TypeId; }
    }
}