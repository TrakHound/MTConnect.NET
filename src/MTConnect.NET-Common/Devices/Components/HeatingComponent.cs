// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Heating is a System used to deliver controlled amounts of heat to achieve a target temperature at a specified heating rate.
    /// </summary>
    public class HeatingComponent : Component 
    {
        public const string TypeId = "Heating";
        public const string NameId = "heat";
        public new const string DescriptionText = "Heating is a System used to deliver controlled amounts of heat to achieve a target temperature at a specified heating rate.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public HeatingComponent()  { Type = TypeId; }
    }
}