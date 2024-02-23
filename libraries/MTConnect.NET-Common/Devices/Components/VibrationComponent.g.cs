// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_4_45f01b9_1643678730400_947692_1640

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of a sensor or an instrument that measures the amount and/or frequency of vibration within a system.Vibration was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Displacement, Frequency etc.
    /// </summary>
    public class VibrationComponent : Component
    {
        public const string TypeId = "Vibration";
        public const string NameId = "vibration";
        public new const string DescriptionText = "Component composed of a sensor or an instrument that measures the amount and/or frequency of vibration within a system.Vibration was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Displacement, Frequency etc.";

        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public VibrationComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}