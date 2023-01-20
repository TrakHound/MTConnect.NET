// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.
    /// </summary>
    public class CoolingComponent : Component 
    {
        public const string TypeId = "Cooling";
        public const string NameId = "cooling";
        public new const string DescriptionText = "Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public CoolingComponent()  { Type = TypeId; }
    }
}