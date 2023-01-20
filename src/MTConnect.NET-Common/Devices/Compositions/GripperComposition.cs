// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public class GripperComposition : Composition 
    {
        public const string TypeId = "GRIPPER";
        public const string NameId = "grip";
        public new const string DescriptionText = "A mechanism that holds a part, stock material, or any other item in place.";

        public override string TypeDescription => DescriptionText;


        public GripperComposition()  { Type = TypeId; }
    }
}
