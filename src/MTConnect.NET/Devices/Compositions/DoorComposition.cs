// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public class DoorComposition : Composition 
    {
        public const string TypeId = "DOOR";
        public const string NameId = "door";
        public new const string DescriptionText = "A mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.";

        public override string TypeDescription => DescriptionText;


        public DoorComposition()  { Type = TypeId; }
    }
}
