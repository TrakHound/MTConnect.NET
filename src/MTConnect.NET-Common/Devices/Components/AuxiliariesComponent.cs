// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliaries organizes Auxiliary component types.
    /// </summary>
    public class AuxiliariesComponent : Component 
    {
        public const string TypeId = "Auxiliaries";
        public const string NameId = "aux";
        public new const string DescriptionText = "Auxiliaries organizes Auxiliary component types.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public AuxiliariesComponent()  { Type = TypeId; }
    }
}