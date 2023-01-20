// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A surface for holding an object or material
    /// </summary>
    public class TableComposition : Composition 
    {
        public const string TypeId = "TABLE";
        public const string NameId = "tab";
        public new const string DescriptionText = "A surface for holding an object or material";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public TableComposition()  { Type = TypeId; }
    }
}