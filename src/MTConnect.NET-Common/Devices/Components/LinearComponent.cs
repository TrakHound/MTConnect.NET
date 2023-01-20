// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// A Linear axis represents prismatic motion along a fixed axis.
    /// </summary>
    public class LinearComponent : Component 
    {
        public const string TypeId = "Linear";
        public const string NameId = "lin";
        public new const string DescriptionText = "A Linear axis represents prismatic motion along a fixed axis.";

        public override string TypeDescription => DescriptionText;


        public LinearComponent()  { Type = TypeId; }
    }
}