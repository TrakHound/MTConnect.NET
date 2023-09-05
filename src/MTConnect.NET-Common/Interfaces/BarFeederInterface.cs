// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between a bar feeder and another piece of equipment.
    /// Bar feeder is a piece of equipment that pushes bar stock(i.e., long pieces of material of various shapes) into an associated piece of equipment – most typically a lathe or turning center.
    /// </summary>
    public class BarFeederInterface : Interface 
    {
        public const string TypeId = "BarFeederInterface";
        public const string NameId = "barFeeder";
        public new const string DescriptionText = "Interface that coordinates the operations between a bar feeder and another piece of equipment. Bar feeder is a piece of equipment that pushes bar stock(i.e., long pieces of material of various shapes) into an associated piece of equipment – most typically a lathe or turning center.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public BarFeederInterface()
        {
            Type = TypeId;
        }
    }
}