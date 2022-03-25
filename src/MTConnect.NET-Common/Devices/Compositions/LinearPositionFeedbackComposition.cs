// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that measures linear motion or position.
    /// </summary>
    public class LinearPositionFeedbackComposition : Composition 
    {
        public const string TypeId = "LINEAR_POSITION_FEEDBACK";
        public const string NameId = "linposfback";
        public new const string DescriptionText = "A mechanism that measures linear motion or position.";

        public override string TypeDescription => DescriptionText;


        public LinearPositionFeedbackComposition()  { Type = TypeId; }
    }
}
