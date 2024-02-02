// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.
    /// </summary>
    public class GalvanomotorComposition : Composition 
    {
        public const string TypeId = "GALVANOMOTOR";
        public const string NameId = "galvanomotorComposition";
        public new const string DescriptionText = "Composition composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public GalvanomotorComposition()  { Type = TypeId; }
    }
}