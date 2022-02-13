// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Sensor is is an Auxiliary that represents the information for a piece of equipment that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.
    /// </summary>
    public class SensorComponent : Component 
    {
        public const string TypeId = "Sensor";
        public const string NameId = "sen";
        public new const string DescriptionText = "Sensor is is an Auxiliary that represents the information for a piece of equipment that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.";

        public override string TypeDescription => DescriptionText;


        public SensorComponent()  { Type = TypeId; }
    }
}
