// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a substance or structure that allows liquids or gases to pass through to remove suspended impurities or to recover solids.
    /// </summary>
    public class FilterComposition : Composition 
    {
        public const string TypeId = "FILTER";
        public const string NameId = "filterComposition";
        public new const string DescriptionText = "Composition composed of a substance or structure that allows liquids or gases to pass through to remove suspended impurities or to recover solids.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public FilterComposition()  { Type = TypeId; }
    }
}