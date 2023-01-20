// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axis is an abstract Component that represents linear or rotational motion for a piece of equipment.
    /// </summary>
    public class AxisComponent : Component 
    {
        public const string TypeId = "Axis";
        public const string NameId = "axis";
        public new const string DescriptionText = "Axis is an abstract Component that represents linear or rotational motion for a piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public AxisComponent()  { Type = TypeId; }
    }
}