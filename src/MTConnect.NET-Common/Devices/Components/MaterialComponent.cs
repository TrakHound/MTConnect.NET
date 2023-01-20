// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of material that is consumed or used by the piece of equipment for production of parts, materials, or other types of goods.
    /// </summary>
    public class MaterialComponent : Component 
    {
        public const string TypeId = "Material";
        public const string NameId = "matertial";
        public new const string DescriptionText = "Component composed of material that is consumed or used by the piece of equipment for production of parts, materials, or other types of goods.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public MaterialComponent()  { Type = TypeId; }
    }
}