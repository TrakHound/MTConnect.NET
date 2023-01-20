// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// A Rotary axis represents rotation about a fixed axis.
    /// </summary>
    public class RotaryComponent : Component 
    {
        public const string TypeId = "Rotary";
        public const string NameId = "rot";
        public new const string DescriptionText = "A Rotary axis represents rotation about a fixed axis.";

        public override string TypeDescription => DescriptionText;


        public RotaryComponent()  { Type = TypeId; }
    }
}
