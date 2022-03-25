// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Actuator is a Component that represents the information for an apparatus for moving or controlling a mechanism or system.
    /// It takes energy usually provided by air, electric current, or liquid and converts the energy into some kind of motion.
    /// </summary>
    public class ActuatorComponent : Component 
    {
        public const string TypeId = "Actuator";
        public const string NameId = "act";
        public new const string DescriptionText = "Actuator is a Component that represents the information for an apparatus for moving or controlling a mechanism or system. It takes energy usually provided by air, electric current, or liquid and converts the energy into some kind of motion.";

        public override string TypeDescription => DescriptionText;


        public ActuatorComponent()  { Type = TypeId; }
    }
}
