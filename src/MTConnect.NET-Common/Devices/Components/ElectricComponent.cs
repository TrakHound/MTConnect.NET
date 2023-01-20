// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment.
    /// The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.
    /// </summary>
    public class ElectricComponent : Component 
    {
        public const string TypeId = "Electric";
        public const string NameId = "elec";
        public new const string DescriptionText = "Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment. The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public ElectricComponent()  { Type = TypeId; }
    }
}