// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a surface that holds an object or material.
    /// </summary>
    public class TableComposition : Composition 
    {
        public const string TypeId = "TABLE";
        public const string NameId = "tableComposition";
        public new const string DescriptionText = "Composition composed of a surface that holds an object or material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public TableComposition()  { Type = TypeId; }
    }
}