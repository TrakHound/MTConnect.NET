// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for turning on or off an electric current or for making or breaking a circuit.
    /// </summary>
    public class SwitchComposition : Composition 
    {
        public const string TypeId = "SWITCH";
        public const string NameId = "sw";
        public new const string DescriptionText = "A mechanism for turning on or off an electric current or for making or breaking a circuit.";

        public override string TypeDescription => DescriptionText;


        public SwitchComposition()  { Type = TypeId; }
    }
}
