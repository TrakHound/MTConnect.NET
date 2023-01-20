// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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