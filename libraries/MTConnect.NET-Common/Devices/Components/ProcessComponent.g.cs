// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605547261014_920934_161

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component composed of a manufacturing process being executed on a piece of equipment.
    /// </summary>
    public abstract class ProcessComponent : Component
    {
        public const string TypeId = "Process";
        public const string NameId = "processComponent";
        public new const string DescriptionText = "Abstract Component composed of a manufacturing process being executed on a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ProcessComponent() { Type = TypeId; }
    }
}