// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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