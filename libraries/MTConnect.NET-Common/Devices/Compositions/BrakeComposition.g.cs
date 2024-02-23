// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738862_411543_44682

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that slows down or stops a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
    /// </summary>
    public class BrakeComposition : Composition 
    {
        public const string TypeId = "BRAKE";
        public const string NameId = "brakeComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that slows down or stops a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BrakeComposition()  { Type = TypeId; }
    }
}