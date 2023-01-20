// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public class MotorComposition : Composition 
    {
        public const string TypeId = "MOTOR";
        public const string NameId = "motor";
        public new const string DescriptionText = "A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.";

        public override string TypeDescription => DescriptionText;


        public MotorComposition()  { Type = TypeId; }
    }
}