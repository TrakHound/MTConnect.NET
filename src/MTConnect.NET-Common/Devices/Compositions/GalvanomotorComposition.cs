// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An electromechanical actuator that produces deflection of a beam of light
    /// or energy in response to electric current through its coil in a magnetic field.
    /// </summary>
    public class GalvanomotorComposition : Composition 
    {
        public const string TypeId = "GALVANOMOTOR";
        public const string NameId = "gmotor";
        public new const string DescriptionText = "An electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public GalvanomotorComposition()  { Type = TypeId; }
    }
}
