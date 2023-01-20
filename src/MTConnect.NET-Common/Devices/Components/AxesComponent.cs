// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axes organizes Axis component types.
    /// </summary>
    public class AxesComponent : Component 
    {
        public const string TypeId = "Axes";
        public const string NameId = "axes";
        public new const string DescriptionText = "Axes organizes Axis component types.";

        public override string TypeDescription => DescriptionText;


        public AxesComponent()  { Type = TypeId; }
    }
}