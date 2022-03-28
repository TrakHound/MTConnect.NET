// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Materials provides information about materials or other items consumed or used by the piece of equipment
    /// for production of parts, materials, or other types of goods. Materials also represents parts or part stock
    /// that are present at a piece of equipment or location to which work is applied to transform the part or stock material into a more finished state.
    /// </summary>
    public class MaterialsComponent : Component 
    {
        public const string TypeId = "Materials";
        public const string NameId = "materials";
        public new const string DescriptionText = "Materials provides information about materials or other items consumed or used by the piece of equipment for production of parts, materials, or other types of goods. Materials also represents parts or part stock that are present at a piece of equipment or location to which work is applied to transform the part or stock material into a more finished state.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public MaterialsComponent()  { Type = TypeId; }
    }
}
