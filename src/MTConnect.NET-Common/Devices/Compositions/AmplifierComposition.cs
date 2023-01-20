// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An electronic component or circuit for amplifying power, electric current, or voltage.
    /// </summary>
    public class AmplifierComposition : Composition 
    {
        public const string TypeId = "AMPLIFIER";
        public const string NameId = "amp";
        public new const string DescriptionText = "An electronic component or circuit for amplifying power, electric current, or voltage.";

        public override string TypeDescription => DescriptionText;


        public AmplifierComposition()  { Type = TypeId; }
    }
}
