// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between two pieces of equipment, one of which controls the operation of a chuck.
    /// The piece of equipment that is controlling the chuck MUST provide the data item  ChuckState as part of the set of information provided.
    /// </summary>
    public class ChuckInterface : Interface 
    {
        public const string TypeId = "ChuckInterface";
        public const string NameId = "chuck";
        public new const string DescriptionText = "Interface that coordinates the operations between two pieces of equipment, one of which controls the operation of a chuck. The piece of equipment that is controlling the chuck MUST provide the data item  ChuckState as part of the set of information provided.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public ChuckInterface()
        {
            Type = TypeId;
        }
    }
}