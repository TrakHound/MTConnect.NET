// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Any substance or structure through which liquids or gases are passed to remove suspended impurities or to recover solids.
    /// </summary>
    public class FilterComposition : Composition 
    {
        public const string TypeId = "FILTER";
        public const string NameId = "fltr";
        public new const string DescriptionText = "Any substance or structure through which liquids or gases are passed to remove suspended impurities or to recover solids.";

        public override string TypeDescription => DescriptionText;


        public FilterComposition()  { Type = TypeId; }
    }
}