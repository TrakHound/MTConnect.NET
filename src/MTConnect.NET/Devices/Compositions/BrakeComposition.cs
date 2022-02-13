// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for slowing or stopping a moving object by the absorption or
    /// transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
    /// </summary>
    public class BrakeComposition : Composition 
    {
        public const string TypeId = "BRAKE";
        public const string NameId = "brake";
        public new const string DescriptionText = "A mechanism for slowing or stopping a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.";

        public override string TypeDescription => DescriptionText;


        public BrakeComposition()  { Type = TypeId; }
    }
}
