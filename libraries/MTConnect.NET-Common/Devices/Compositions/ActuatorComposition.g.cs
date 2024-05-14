// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738859_525340_44674

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that moves or controls a mechanical part of a piece of equipment.It takes energy usually provided by air, electric current, or liquid and converts the energy into some kind of motion.
    /// </summary>
    public class ActuatorComposition : Composition 
    {
        public const string TypeId = "ACTUATOR";
        public const string NameId = "actuatorComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that moves or controls a mechanical part of a piece of equipment.It takes energy usually provided by air, electric current, or liquid and converts the energy into some kind of motion.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ActuatorComposition()  { Type = TypeId; }
    }
}