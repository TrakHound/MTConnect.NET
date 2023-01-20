// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for moving or controlling a mechanical part of a piece of equipment.
    /// </summary>
    public class ActuatorComposition : Composition 
    {
        public const string TypeId = "ACTUATOR";
        public const string NameId = "act";
        public new const string DescriptionText = "A mechanism for moving or controlling a mechanical part of a piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public ActuatorComposition()  { Type = TypeId; }
    }
}