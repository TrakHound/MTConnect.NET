// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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