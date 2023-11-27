// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An electromechanical actuator that produces deflection of a beam of light
    /// or energy in response to electric current through its coil in a magnetic field.
    /// </summary>
    public class GalvanomotorModel : CompositionModel, IGalvanomotorModel
    {
        public GalvanomotorModel() 
        {
            Type = GalvanomotorComposition.TypeId;
        }

        public GalvanomotorModel(string compositionId)
        {
            Id = compositionId;
            Type = GalvanomotorComposition.TypeId;
        }
    }
}