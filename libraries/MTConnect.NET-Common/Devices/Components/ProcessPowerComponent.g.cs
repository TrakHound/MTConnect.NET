// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382012_290973_42279

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of a power source associated with a piece of equipment that supplies energy to the manufacturing process separate from the Electric system.
    /// </summary>
    public class ProcessPowerComponent : Component
    {
        public const string TypeId = "ProcessPower";
        public const string NameId = "processPowerComponent";
        public new const string DescriptionText = "System composed of a power source associated with a piece of equipment that supplies energy to the manufacturing process separate from the Electric system.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ProcessPowerComponent() { Type = TypeId; }
    }
}