// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

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

        public GalvanomotorComposition()  { Type = TypeId; }
    }
}
