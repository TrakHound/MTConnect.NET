// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public SensorComponent()  { Type = TypeId; }
    }
}