// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
