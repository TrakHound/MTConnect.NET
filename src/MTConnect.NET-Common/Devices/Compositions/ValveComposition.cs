// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Any mechanism for halting or controlling the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.
    /// </summary>
    public class ValveComposition : Composition 
    {
        public const string TypeId = "VALVE";
        public const string NameId = "valve";
        public new const string DescriptionText = "Any mechanism for halting or controlling the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.";

        public override string TypeDescription => DescriptionText;


        public ValveComposition()  { Type = TypeId; }
    }
}