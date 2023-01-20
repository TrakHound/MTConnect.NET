// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// BarFeeder is a Loader involved in delivering bar stock to a piece of equipment.
    /// </summary>
    public class BarFeederComponent : Component 
    {
        public const string TypeId = "BarFeeder";
        public const string NameId = "bfeeder";
        public new const string DescriptionText = "BarFeeder is a Loader involved in delivering bar stock to a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public BarFeederComponent()  { Type = TypeId; }
    }
}